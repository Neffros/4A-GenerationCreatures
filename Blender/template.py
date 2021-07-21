#fully functional with mirror but no static bone
import bpy
import bmesh
import random
import struct
from collections import namedtuple
from mathutils import Matrix, Vector
from math import radians

#####
#
# Creature data structure
#
#####

def clean_objects():
    
    bpy.ops.object.mode_set(mode="OBJECT")
    
    for obj in bpy.context.scene.objects:
        obj.select_set(True)

    bpy.ops.object.delete()

clean_objects()

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
leg_meshes = []

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
'''
data = struct.pack(format,
    True,
    4,
    5,
    5,
    1,
    0,
    70,
    0,
    0,
    0
)'''
c = Creature._make(struct.unpack(format, data))

#####
#
# Joint class (merci Maxime)
#
#####

class Joint:
    def __init__(self, pos, armature, parent = None, bone = None, root_bone = None):
        self.pos = pos
        self.armature = armature
        self.parent = parent
        self.bone = bone
        self.root_bone = root_bone
    
    def grow_bone(self, end):
        bpy.ops.object.mode_set(mode='EDIT')
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
            #if first bone
            print("first bone")
            created_bone = self.armature.edit_bones.new('bone1')
            created_bone.head = self.pos
            created_bone.tail = target_pos
            self.root_bone = self.armature.edit_bones[0]
        
        return Joint(target_pos, self.armature, self, created_bone)
    
    def grow_bone_from_root(self, end):
        bpy.ops.object.mode_set(mode='EDIT')
        bpy.ops.armature.extrude_move(ARMATURE_OT_extrude={"forked": False}, TRANSFORM_OT_translate={"value": end})
        created_bone = self.armature.edit_bones[-1]
        return Joint(target_pos, self.armature, self, created_bone)


#####
#
# Generate creature
#
#####


def min(a, b):
    return a if a < b else b

def abs(x):
    return -x if x < 0 else x

def apply_all_modifiers():
    
    bpy.ops.object.mode_set(mode='OBJECT')
    
    for leg in leg_meshes:
        bpy.context.view_layer.objects.active = leg
        bpy.ops.object.modifier_apply(modifier='Skin')
        bpy.ops.object.modifier_apply(modifier='Sub')
        
def join_legs_to_body():
    
    bpy.ops.object.mode_set(mode='OBJECT')
    bpy.ops.object.select_all(action = "DESELECT")
    for leg in leg_meshes:
        bpy.ops.object.select_all(action = "DESELECT")
        cube = bpy.data.objects["Cube"]
        cube.select_set(True)
        bpy.context.view_layer.objects.active = cube
        leg.select_set(True)
        
        
        bpy.ops.object.join()
def join_mesh_to_armature():
    bpy.data.objects["Cube"].select_set(True)
    bpy.data.objects["Armature"].select_set(True)
    arma = bpy.data.objects["Armature"]
    bpy.context.view_layer.objects.active = arma
    bpy.ops.object.parent_set(type='ARMATURE_AUTO')

def generate_legs(data, root_bone):
    leg_joints = [
        Vector((0, 1.7, 2)),
        Vector((0, 5, 0.2)),
        Vector((0, 5, -(data.scale_z / 2 + 4)))
    ]
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
        
        mesh_left = bpy.data.meshes.new("Leg")
        obj_left = bpy.data.objects.new("Leg", mesh_left)
        mesh_right = bpy.data.meshes.new("Leg")
        obj_right = bpy.data.objects.new("Leg", mesh_right)
        # Generate bmesh

        bm_left = bmesh.new()
        bm_right = bmesh.new()
        
        # Root of the leg

        left_root = bm_left.verts.new((
            i * leg_distance + leg_offset.x + data.leg_custom_offset_x,
            -(leg_offset.y + data.leg_custom_offset_y),
            leg_offset.z + data.leg_custom_offset_z
        ))
        
        right_root = bm_right.verts.new((
            i * leg_distance + leg_offset.x + data.leg_custom_offset_x,
            leg_offset.y + data.leg_custom_offset_y,
            leg_offset.z + data.leg_custom_offset_z
        ))
        
        #bone from root to leg root
        left_leg_root_bone = root_bone.grow_bone(left_root.co)
        right_leg_root_bone = root_bone.grow_bone(right_root.co)
        previous_vertice_left = left_root
        previous_vertice_right = right_root
        
        # Loop on joints

        for joint in leg_joints:
            
            # Link joint to previous joint
            
            left_vertice = bm_left.verts.new((
                left_root.co.x + joint.x,
                left_root.co.y - joint.y,
                left_root.co.z + joint.z
            ))
            right_vertice = bm_right.verts.new((
                right_root.co.x + joint.x,
                right_root.co.y + joint.y,
                right_root.co.z + joint.z
            ))
            bm_left.edges.new([previous_vertice_left, left_vertice])
            bm_right.edges.new([previous_vertice_right, right_vertice])
            
            
            # Create bone on joint
            left_leg_root_bone = left_leg_root_bone.grow_bone(left_vertice.co - previous_vertice_left.co)
            right_leg_root_bone = right_leg_root_bone.grow_bone(right_vertice.co - previous_vertice_right.co)
            
            previous_vertice_left = left_vertice
            previous_vertice_right = right_vertice
            ''' Extrude previous bone to vertice location and save bone '''

        # Convert to mesh
        
        bm_left.to_mesh(mesh_left)
        bm_right.to_mesh(mesh_right)
        
        # Assign material
        
        mat_left = bpy.data.materials.new(name="Material")
        mat_right = bpy.data.materials.new(name="Material")
        
        for i in range(3):
            mat_left.diffuse_color[i] = random.random()
            mat_right.diffuse_color[i] = random.random()
        
        obj_left.data.materials.append(mat_left)
        obj_right.data.materials.append(mat_right)
        
        #bpy.ops.object.mode_set(mode='OBJECT')
        # Add skin
        skin_left = obj_left.modifiers.new(name="Skin", type='SKIN')
        skin_right = obj_right.modifiers.new(name="Skin", type='SKIN')
        
        # Add subdivisions
        
        sub_left = obj_left.modifiers.new(name="Sub", type='SUBSURF')
        sub_right = obj_right.modifiers.new(name="Sub", type='SUBSURF')
        sub_left.levels = 4
        sub_right.levels = 4
        
        
        # Link to scene
    
        obj_left.location = (0, 0, 0)
        obj_right.location = (0, 0, 0)
        bpy.context.scene.collection.objects.link(obj_left)
        bpy.context.scene.collection.objects.link(obj_right)

        leg_meshes.append(obj_left)
        leg_meshes.append(obj_right)

def generate_creature(data):

    #bpy.ops.object.mode_set(mode='OBJECT')

    # Create creature body
    
    bpy.ops.mesh.primitive_cube_add(
        location=(0, 0, 0),
        scale=get_creature_scale(data)
    )
    
    body = bpy.context.active_object
    context = bpy.context
    scene = context.scene

    # Create armature
    
    #bpy.ops.object.armature_add(location=creature_location.to_tuple())
    #bpy.ops.object.armature_add(location=(0,0,0))
    #armature = bpy.context.active_object
    armature = bpy.data.armatures.new("Armature")
    armature_obj = bpy.data.objects.new("Armature", armature)
    
    scene.collection.objects.link(armature_obj)
    armature_obj.select_set(False)
    context.view_layer.objects.active = armature_obj
    armature_obj.select_set(True)
    
    #create root bone
    basePos = Vector((0,0,2))
    current_bone = Joint(basePos, armature)
        
    boneCord = Vector((0,0,-2))
    current_bone = current_bone.grow_bone(boneCord)
    root_bone = current_bone

    generate_legs(data, root_bone)
        


        

#####
#
# Main
#
#####


generate_creature(c)
apply_all_modifiers()
join_legs_to_body()
join_mesh_to_armature()

bpy.ops.export_scene.fbx(filepath="FBX_EXPORT_PATH", axis_forward="-Z", axis_up="Y")