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

LEG_AUTO_DISTANCE = True
LEG_NUMBER = 3
SCALE_X = 4
SCALE_Y = 5
SCALE_Z = 2
LEG_WANTED_DISTANCE = 2
LEG_SPREAD_ANGLE = 20 
LEG_CUSTOM_OFFSET_X = 0
LEG_CUSTOM_OFFSET_Y = 0
LEG_CUSTOM_OFFSET_Z = 0

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
    LEG_AUTO_DISTANCE,
    LEG_NUMBER,
    SCALE_X,
    SCALE_Y,
    SCALE_Z,
    LEG_WANTED_DISTANCE,
    LEG_SPREAD_ANGLE,
    LEG_CUSTOM_OFFSET_X,
    LEG_CUSTOM_OFFSET_Y,
    LEG_CUSTOM_OFFSET_Z
)
c = Creature._make(struct.unpack(format, data))

#####
#
# Joint class (merci Maxime)
#
#####

class Joint:
    def __init__(self, pos, armature, parent = None, bone = None):
        self.pos = pos
        self.armature = armature
        self.parent = parent
        self.bone = bone
    
    def grow_bone(self, end):
        target_pos = self.pos + end
        
        if self.parent:
            for bone in self.armature.edit_bones:
                bone.select = False
                bone.select_head = False
                bone.select_tail = False
            self.bone.select_tail = True
            bpy.ops.armature.extrude_move(ARMATURE_OT_extrude={"forked": False}, TRANSFORM_OT_translate={"value": end})
            created_bone = self.armature.edit_bones[-1]
        else:
            created_bone = self.armature.edit_bones.new('bone1')
            created_bone.head = self.pos
            created_bone.tail = target_pos
        
        return Joint(target_pos, self.armature, self, created_bone)

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
    context = bpy.context
    scene = context.scene
    body = bpy.context.active_object
    
    # Create armature
    
    
    
    # Leg pre-computed variables
    
    leg_distance = (data.scale_x - 2 * abs(data.leg_custom_offset_x)) / data.leg_number\
        if data.leg_auto_distance\
        else min(data.leg_wanted_distance, (data.scale_x - 2 * abs(data.leg_custom_offset_x)) / data.leg_number)
    leg_offset = Vector((-leg_distance * (data.leg_number - 1) / 2, data.scale_y / 2, 0))
    leg_angle_increment_value = data.leg_spread_angle / (data.leg_number - 1)
    leg_current_angle = data.leg_spread_angle / 2

    # Generate bmesh

    bm = bmesh.new()
    
    # Generate mesh and Blender object
    
    mesh = bpy.data.meshes.new("Legs")
    obj = bpy.data.objects.new("Legs", mesh)
    bpy.context.scene.collection.objects.link(obj)
    
    # Legs generation loop
    
    for i in range(data.leg_number):
        
        # Root of the leg

        root = bm.verts.new((
            i * leg_distance + leg_offset.x + data.leg_custom_offset_x,
            leg_offset.y + data.leg_custom_offset_y,
            leg_offset.z + data.leg_custom_offset_z
        ))
        
        previous_vertice = root
        
        # Create leg armature
        
        armature = bpy.data.armatures.new("Armature")
        armature_obj = bpy.data.objects.new("Armature", armature)

        scene.collection.objects.link(armature_obj)
        armature_obj.select_set(False)
        context.view_layer.objects.active = armature_obj
        armature_obj.select_set(True)
        
        current_bone = Joint(root.co, armature)

        bpy.ops.object.mode_set(mode='EDIT')
        
        # Loop on joints

        for joint in leg_joints:
            
            # Link joint to previous joint
            
            vertice = bm.verts.new((
                root.co.x + joint.x,
                root.co.y + joint.y,
                root.co.z + joint.z
            ))
            bm.edges.new([previous_vertice, vertice])
            
            # Grow bone to vertice
            
            current_bone = current_bone.grow_bone(vertice.co - previous_vertice.co)
            
            previous_vertice = vertice

        bpy.ops.object.mode_set(mode="OBJECT")
        bpy.ops.object.select_all(action = "DESELECT")
        
        # Link armature to mesh
        
        obj.select_set(True)
        armature_obj.select_set(True)
        bpy.context.view_layer.objects.active = armature_obj
        bpy.ops.object.parent_set(type='ARMATURE_AUTO')
        
        bpy.ops.object.select_all(action = "DESELECT")
        
        # Rotate the leg around its root
        
        '''
        Traduire de bmesh à bpy et à appliquer sur l'armature et non sur le mesh
        
        space = obj.matrix_world.copy()
        space.translation -= root.co
        rotation = Matrix.Rotation(radians(leg_current_angle), 3, (0, 0, 1))
        bmesh.ops.rotate(bm, matrix=rotation, verts=bm.verts, space=space)
        leg_current_angle = leg_current_angle - leg_angle_increment_value
        '''
        
        # Add skin
            
        skin = obj.modifiers.new(name="Skin", type='SKIN')
        
        # Add subdivisions
        
        sub = obj.modifiers.new(name="Sub", type='SUBSURF')
        sub.levels = 4
        
        '''
        # Add mirroring
        
        mirror = obj.modifiers.new(name="Mirror", type='MIRROR')
        mirror.use_axis[0] = False
        mirror.use_axis[1] = True
        '''
        
        # Link to scene
    
        obj.location = (0, 0, 0)
        
        bpy.ops.object.select_all(action = "DESELECT")
    
    # Convert to mesh
    
    bm.to_mesh(mesh)

#####
#
# Main
#
#####

clean_objects()
generate_creature(c)
'''bpy.ops.export_scene.fbx(filepath="FBX_EXPORT_PATH", axis_forward="-Z", axis_up="Y")'''
