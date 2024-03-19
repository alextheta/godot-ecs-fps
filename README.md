# Godot ECS FPS demo
This is a demo project that shows basic concepts of making a game on Godot (using C#) with applying ECS architecture and dependency injection.

## Overview

Here are some features you can find:

0. A first-person controller capable of walking, stepping on stairs, jumping, climbing, and crouching with variable head height (similar to Dishonored, perhaps).
1. Grouping of ECS systems by logical features, which makes the code well-organized.
2. Injection of various data (such as ECS pools and Godot nodes) into systems.
3. Unreal Engine-like (though much more simplified) possession system, where you can mark a specific character with a 'possess' component, causing it to start listening to input.
4. Spawn system that allows you to place spawn points and set a character prefab (resource/scene) to them.
5. Input based on one-frame ECS entities that carry a specific input component.

You can find the entry point in `Service.cs`. This script initializes all the bindings and resolutions for the DI context. All the features are also registered here.

Every feature has its own set of systems, components, and other classes that can be found in the feature folder under the `Features` directory.

The `Common` folder contains Godot-ECS bindings that help keep the two worlds consistent.

Also, in the `CleanupFeature`, you can see the approach to creating ECS events made by one-frame entities that carry a component with the `Event` postfix in the name. The `CleanupFeature` removes them with `CleanupEventFeature<T>` systems when the frame ends. That's why this feature should be last in the feature registration list. Also, you can't use this approach in `PhysicsUpdate` because it is not synchronized with the game loop, and your target systems may be executed after the `CleanupFeature`'s pass.

## Dependencies

* [LeoECS Lite](https://github.com/Leopotam/ecslite) — The ECS framework used in the project
* [tenject](https://github.com/alextheta/tenject) - A simple C# DI-container
* [ECS feature runner](https://github.com/alextheta/ecs-feature-runner) - A helper to organize ECS features and make it a kind of modular design

Stair movement is an adaptation of [JheKWall's stair step demo](https://github.com/JheKWall/Godot-Stair-Step-Demo)

Also, I use [godot-jolt](https://github.com/godot-jolt/godot-jolt), but it is not included as a dependency. Everything should be fine with the default physics engine, though it is not guaranteed

## Known bugs

* Due to the stair-stepping mechanism, the default CharacterController3D's slowdown on slopes is not functioning correctly. This issue can likely be fixed by verifying the collision normal.
* Stair-stepping doesn't work when crouching.
* The camera may behave strangely when climbing in tight spaces and requiring crouching.