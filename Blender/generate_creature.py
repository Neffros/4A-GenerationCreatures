import bpy
import bmesh
from mathutils import Matrix, Vector
from math import radians

def clean_objects():
    bpy.ops.object.mode_set(mode='OBJECT')
    
    for obj in bpy.context.scene.objects:
        obj.select_set(True)

    bpy.ops.object.delete()

def min(a, b):
    return a if a < b else b

def abs(x):
    return -x if x < 0 else x
    
def selection():
    return

def init_population(g):
    return

def evaluate_population(g): #get fitness value
    return

def mutation(g):
    return

def evaluate(g):
    return

def recombine(g):
    return

def generate_generations():
    population = init_population(0)
    population = evaluate_population(0)
    for i in range(1, 50):
        population = selection(i)
        population = recombine(i)
        population = mutation(i)
        population = evaluate(i)
    
    #generate_creatre(population???)
    return 
def generate_creature(
    creature_location = Vector((0, 0, 0)),
    creature_scale = Vector((15, 3, 4)),
    leg_number = 8,
    leg_wanted_distance = -1,
    leg_spread_angle = 60,
    leg_joints = [
        Vector((0, 1.7, 2)),
        Vector((0, 5, 0.2)),
        Vector((0, 5, -(creature_scale.z / 2 + 4)))
    ],
    leg_custom_offset = Vector((-2, 1, 2))
):
    
    bpy.ops.object.mode_set(mode='OBJECT')

    # Create creature body
    
    bpy.ops.mesh.primitive_cube_add(
        location=creature_location.to_tuple(),
        scale=creature_scale.to_tuple()
    )
    
    body = bpy.context.active_object
    
    # Create armature
    
    '''bpy.ops.object.armature_add(location=creature_location.to_tuple())
    armature = bpy.context.active_object'''
    
    # Leg pre-computed variables
    
    leg_distance = (creature_scale.x - 2 * abs(leg_custom_offset.x)) / leg_number\
        if leg_wanted_distance < 0\
        else min(leg_wanted_distance, (creature_scale.x - 2 * abs(leg_custom_offset.x)) / leg_number)
    leg_offset = Vector((-leg_distance * (leg_number - 1) / 2, creature_scale.y / 2, 0))
    leg_angle_increment_value = leg_spread_angle / (leg_number - 1)
    leg_current_angle = leg_spread_angle / 2
    
    # Legs generation loop
    
    for i in range(leg_number):
        
        # Generate mesh and Blender object
        
        mesh = bpy.data.meshes.new("Leg")
        obj = bpy.data.objects.new("Leg", mesh)

        # Generate bmesh

        bm = bmesh.new()
        
        # Root of the leg

        root = bm.verts.new((
            i * leg_distance + leg_offset.x + leg_custom_offset.x,
            leg_offset.y + leg_custom_offset.y,
            leg_offset.z + leg_custom_offset.z
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
    
        obj.location = creature_location.to_tuple()
        bpy.context.scene.collection.objects.link(obj)

a = 7
bin = f'{a:08b}'
print(bin)
#generate_creature()