# Controllable - TrafficCone

This repository contains the code and assets for a custom Controllable Plugin - TrafficCone.

To use this Controllable Plugin:

1) Clone the repo into Assets/External/Controllables/TrafficCone inside of your Simulator Unity Project

2) Build the Controllable Plugin for use with the Simulator, navigate to the `Simulator -> Build Controllables` Unity Editor menu item. Select TrafficCone controllable and build.  Output bundle will be in AssetBundles/Controllables folder in root of Simulator Unity Project

3) Simulator will load, at runtime, all custom Controllable Plugin bundles in AssetBundles/Controllables directory



## Custom Logic

To implement custom logic, contained in a given Controllable Plugin project there must be an IControllable<Type> implementation. An example of this is in TrafficCone.cs

The interface requires the following to be implemented:

```c#
public bool Spawned { get; set; }        
public string UID { get; set; }
public string ControlType { get; set; } = "type"
public string CurrentState { get; set; }
public string[] ValidStates { get; } = new string[]{};
public string[] ValidActions { get; } = new string[]{};

// Control policy defines rules for control actions
public string DefaultControlPolicy { get; set; }
public string CurrentControlPolicy { get; set; }

public Control(List<ControlAction> controlActions)
{
    //
}
```

On ```Awake()``` CurrentControlPolicy and CurrentState must be set, e.g.

```c#
private void Awake()
{
    CurrentControlPolicy = DefaultControlPolicy;
    CurrentState = "";
}
```

Control checks the parsed ControlActions and sets the CurrentState

```c#
public void Control(List<ControlAction> controlActions)
{
    for (int i = 0; i < controlActions.Count; i++)
    {
        var action = controlActions[i].Action;
        var value = controlActions[i].Value;

        switch (action)
        {
            case = "exampleAction"
            	CurrentState = value; // not used for traffic cone
            	break;
            default:
            Debug.LogError($"'{action}' is an invalid action for '{ControlType}'");
                break;
        }
    }
}
```



## Python API example

Each controllable object has its own `valid actions` (e.g., green, yellow, red, trigger, wait, loop, on, off, "") that it can take and is controlled based on `control policy`, which defines rules for control actions.

To get a list of controllable objects in a scene:

```python
controllables = sim.get_controllables()
```

For a controllable object of interest, you can get following information:

```python
cone = controllables[0]
print("Type:", cone.type)
print("Transform:", cone.transform)
print("Current state:", cone.current_state)
print("Valid actions:", cone.valid_actions)
```

For control policy, each controllable object always has default control policy (read-only). When you load a scene for the first time or reset a scene to the initial state, a controllable object resets current control policy to default one follows it.

You can get default control policy and current control policy as follows:

```python
print("Default control policy:", cone.default_control_policy)
print("Current control policy:", cone.control_policy)
```

To change a current control policy, you can create a new control policy and call `control` function as below:

```python
control_policy = "on"
signal.control(control_policy)
```

To add a plugin controllable and set object state

```python
state = lgsvl.ObjectState()
state.transform.position = lgsvl.Vector(0,0,0)
state.transform.rotation = lgsvl.Vector(0,0,0)
state.velocity = lgsvl.Vector(0,10,0)
state.angular_velocity = lgsvl.Vector(6.5,0,0)

cone = sim.controllable_add("TrafficCone", state)
```

To get plugin controllable object state

```python
cone.object_state
```

To set plugin controllable object state

```python
state = lgsvl.ObjectState()
state.transform.position = lgsvl.Vector(0, 0, -10)
cone.object_state = state
```

Controllables can also have a Unity RigidBody component at the root and apply velocity from the API. 

```python
#!/usr/bin/env python3
#
# Copyright (c) 2020 LG Electronics, Inc.
#
# This software contains code licensed as described in LICENSE.
#

import os
import lgsvl

sim = lgsvl.Simulator(os.environ.get("SIMULATOR_HOST", "127.0.0.1"), 8181)

scene_name = "CubeTown"

if sim.current_scene == scene_name:
  sim.reset()
else:
  sim.load(scene_name, 42)

spawns = sim.get_spawn()

state = lgsvl.AgentState()
forward = lgsvl.utils.transform_to_forward(spawns[0])
right = lgsvl.utils.transform_to_right(spawns[0])
up = lgsvl.utils.transform_to_up(spawns[0])
state.transform = spawns[0]

ego = sim.add_agent("Lincoln2017MKZ (Apollo 5.0)", lgsvl.AgentType.EGO, state)

print("Python API Quickstart #28: How to Add/Control Traffic Cone")

for i in range(10*3):
  # Create controllables in a block
  start = spawns[0].position + (5 + (1.0 * (i//6))) * forward - (2 + (1.0 * (i % 6))) * right
  end = start + 10 * forward

  state = lgsvl.ObjectState()
  state.transform.position = start
  state.transform.rotation = spawns[0].rotation
  # Set velocity and angular_velocity
  state.velocity = 10 * up
  state.angular_velocity = 6.5 * right
  
  # add controllable
  o = sim.controllable_add("TrafficCone", state)
  

print("\nAdded {} Traffic Cones".format(i + 1))

seconds = 10
input("\nPress Enter to run simulation for {} seconds".format(seconds))
print("\nRunning simulation for {} seconds...".format(seconds))
sim.run(seconds)

print("\nDone!")
```



### Copyright and License

Copyright (c) 2020 LG Electronics, Inc.

This software contains code licensed as described in LICENSE.
