import bpy
import bmesh
import struct
from collections import namedtuple
from mathutils import Matrix, Vector
from math import radians

#####
#
# Creature data structure
#
#####

names = ' '.join([
    'leg_auto_distance',
    'leg_number',
    'scale_x',
    'scale_y',
    'scale_z',
    'leg_wanted_distance',
    'leg_spread_angle',
    'leg_custom_offset_x',
    'leg_custom_offset_y',
    'leg_custom_offset_z',
])
format = '?hffffffff'
Creature = namedtuple('Creature', names)

def get_creature_scale(data):
    return (data.scale_x, data.scale_y, data.scale_z)

#####
#
# Create data
#
#####

data = struct.pack(format,
    False,
    3,
    5,
    2,
    3,
    1,2,
    40,
    0,
    0,
    0
)
c = Creature._make(struct.unpack(format, data))

#####
#
# Generate creature
#
#####

def clean_objects():
    for obj in bpy.context.scene.objects:
        obj.select_set(True)

    bpy.ops.object.delete()

def min(a, b):
    return a if a < b else b

def abs(x):
    return -x if x < 0 else x

def generate_creature(data):
    leg_joints = [
        Vector((0, 1.7, 2)),
        Vector((0, 5, 0.2)),
        Vector((0, 5, -(data.scale_z / 2 + 4)))
    ]
    #bpy.ops.object.mode_set(mode='OBJECT')

    # Create creature body
    
    bpy.ops.mesh.primitive_cube_add(
        location=(0, 0, 0),
        scale=get_creature_scale(data)
    )
    
    body = bpy.context.active_object
    
    # Create armature
    
    '''bpy.ops.object.armature_add(location=creature_location.to_tuple())
    armature = bpy.context.active_object'''
    
    # Leg pre-computed variables
    
    leg_distance = (data.scale_x - 2 * abs(data.leg_custom_offset_x)) / data.leg_number\
        if data.leg_auto_distance\
        else min(data.leg_wanted_distance, (data.scale_x - 2 * abs(data.leg_custom_offset_x)) / data.leg_number)
    leg_offset = Vector((-leg_distance * (data.leg_number - 1) / 2, data.scale_y / 2, 0))
    leg_angle_increment_value = data.leg_spread_angle / (data.leg_number - 1)
    leg_current_angle = data.leg_spread_angle / 2
    
    # Legs generation loop
    
    for i in range(data.leg_number):
        
        # Generate mesh and Blender object
        
        mesh = bpy.data.meshes.new("Leg")
        obj = bpy.data.objects.new("Leg", mesh)

        # Generate bmesh

        bm = bmesh.new()
        
        # Root of the leg

        root = bm.verts.new((
            i * leg_distance + leg_offset.x + data.leg_custom_offset_x,
            leg_offset.y + data.leg_custom_offset_y,
            leg_offset.z + data.leg_custom_offset_z
        ))
        
        previous_vertice = root
        
        # Create bone on root
        
        ''' Extrude armature to root location and save bone '''
        
        # Loop on joints

        for joint in leg_joints:
            
            # Link joint to previous joint
            
            vertice = bm.verts.new((
                root.co.x + joint.x,
                root.co.y + joint.y,
                root.co.z + joint.z
            ))
            bm.edges.new([previous_vertice, vertice])
            previous_vertice = vertice
            
            # Create bone on joint
            
            ''' Extrude previous bone to vertice location and save bone '''
            
        # Rotate the leg around its root
        
        space = obj.matrix_world.copy()
        space.translation -= root.co
        rotation = Matrix.Rotation(radians(leg_current_angle), 3, (0, 0, 1))
        bmesh.ops.rotate(bm, matrix=rotation, verts=bm.verts, space=space)
        leg_current_angle = leg_current_angle - leg_angle_increment_value
    
        # Convert to mesh
        
        bm.to_mesh(mesh)
        
        # Add skin
            
        skin = obj.modifiers.new(name="Skin", type='SKIN')
        
        # Add subdivisions
        
        sub = obj.modifiers.new(name="Sub", type='SUBSURF')
        sub.levels = 4
        
        # Add mirroring
        
        mirror = obj.modifiers.new(name="Mirror", type='MIRROR')
        mirror.use_axis[0] = False
        mirror.use_axis[1] = True
    
        # Link to scene
    
        obj.location = (0, 0, 0)
        bpy.context.scene.collection.objects.link(obj)

#####
#
# Main
#
#####

clean_objects()
generate_creature(c)
bpy.ops.export_scene.fbx(filepath="E:/Unity/4A-GenerationCreatures/Assets\Prefabs/creature.fbx", axis_forward="-Z", axis_up="Y")