
# Zds.Flow

A library to control the flow of program is a completely different way. It puts life to the program. This library can be used as partial replacement of old Asynchronous system in .Net.

You can design applications with massive parallelism without dealing with thread safety. You can create applications completely detached from UI. You can easily and safely interact with it.
  
There are four major namespaces in library:
  
- Updaters (Objects that create threads, Acts as ThreadPool)
- Updatables (Objects that use threads created by Updaters)
- Collections (Contains different Thread Safe/Unsafe collections)
- Machinery (Used to design a graph that controls the flow of objects)
  
Other namespaces:
  
- DelayHandling (Objects mostly used by Updates to block one thread to allow the flow of other threads)
- ExceptionHandling (Objects for handling exceptions)
- Stopwatch (Objects for tracking time)
- Interfaces (Interfaces that do not fit in any category)
  
# Things to Remember
  
1. Every objects that implements `IStartStopable` interface will not work unless you `Start` it. Every Updater/Updatable object implements `IStartStopable` interface. So you have to start them manually. You can stop them by calling `Stop`. You can check object status by using `IStartStopable.IsRunning` property.
2. An `Updatable` object will never work without an `Updater`. By default there is a global `Updater` that is assigned to `Updatable` if `Updater` is not given. You can assign an `Updater` to an `Updateable` by setting `Updatable.Updater` or calling `Updater.Add()`. You can get all `Updatables` assigned to an `Updater` by using `Updater.Targets` property.
3. A thread safe collection has prefix `Safe` in its name. So be careful when using collections in Asynchronous objects or in a `Machinery`.
4. Objects that have managed/unmanaged resources implement `IDestroyable`. These objects have method `Destroy()`. When you call it, the object will release its resources and will stop functioning. You can check whatever an object is destroyed or not by using `IDestroyable.IsDestroyed` property.
5. Objects that throw exceptions implement `IThrowsException` interface. These objects have event `OnException`. You can use this event to keep track of exceptions. You can even create an `ExceptionHandler` and add your `IThrowsException` object to it.
  
# Updatables

Updatables are objects that implement `IUpdatable`. These objects have method `Update()` that is recursively called by an `Updater` assigned to it. It is not fixed how fast or delayed the method will be called. So, it should be handled by `Updatable`. All `Updatables` implement `IDestroyable`  `IThrowsException` and `IStartStopable`.
  
Every object in `Updatable` namespace has at least one overridable object that can be overridden to define the behaviour of object. You can also do it by passing pointer of a method of same signature through constructor to define behaviour. You can also use associated event. So, inheritance is not MUST.
  
Here is the list of all `Updatables` and their functionalities.
  
|Updatables|Functionality|
|--|--|
|Updatable|Its method `OnUpdated` is called recursively. `OnUpdated` is an overridable method. You can inherit the object to define what to do. You can also define what to do by passing pointer to method matching the signature of `OnUpdated` or using `PreUpdate` or `PostUpdate` events. `PreUpdate` is called before `OnUpdated` is called and `PostUpdate` is called after `OnUpdated` is called.|
|SyncObject|Inherits `Updatable` and overrides method `OnUpdated`. Its method `SyncUpdate` is called recursively such that only one thread is in `SyncUpdate` method at a time. You can get lock by using `Lock` property.|
|AsyncObject|Inherits `Updatable` and overrides method `OnUpdated`. Its method `AsyncUpdate` is recursively called by unspecific number of threads. That number depends on multiple factors e.g `Updater`. The number of threads will not exceed `MaxThread` property of `AsyncObject`. You can get total threads currently stuck in `AsyncUpdate` by using `ActiveThreads` property.|
|SyncTimer|Inherits `SyncObject` and overrides method `SyncUpdate`. Its method `Tick` is called periodically after `SyncTimer.Delay` time such that only one thread is in `Tick` method. Next tick time does not depend on previous tick time but you can make timer tolerant by enabling `SyncTimer.IsTolerant`. In each tick timer calculates error (difference between actual tick time and expected tick time), if `SyncTimer.ErrorCorrection` is enabled this time will be subtracted from next tick time of timer. You can force a `Tick` at any instance by calling `SyncTimer.TickNow()`.|
|AsyncTimer|Inherits `AsyncObject` and overrides method `AsyncUpdate` method. Its method `Tick` is called periodically after `AsyncTimer.Delay` time by unspecific number of threads. Next tick time does not depend on previous tick time but you can make timer tolerant by enabling `AsyncTimer.IsTolerant`. In each tick timer calculates error (difference between actual tick time and expected tick time), if `AsyncTimer.ErrorCorrection` is enabled this time will be subtracted from next tick time of timer. You can force a `Tick` at any instance by calling `AsyncTimer.TickNow()`.|
|RigidStateMachine|Inherits `SyncTimer` and overrides `Tick` method. Method `Machine` is called when `Tick` is called. Method `Machine` takes object state (by ref) as parameter and supposed to update the argument to determine the next state of object. Object current state can be determined/modified by using `RigidStateMachine.State` property.|
|RigidAnimation|Inherits `RigidStateMachine` and overrides `Machine` method. Its `StateType` is `Decimal` when determines the position of animation (`RigidAnimation.State`). Method `Animate` is called when `Machine` is called. Method `Animate` takes animation position (by ref) as argument and is supposed to perform action based on animation position. The value of animation position is automatically  updated. You can change `RigidAnimation.Delta` to change the rate of change of animation position per `RigidAnimation.Delay` time. Animation will stop once animation position reaches one and will reset its value to zero. You can enable `RigidAnimation.IsLoop` to create looping animation. You can enable `RigidAnimation.FrameSkipping` to skip frame if last `Animate` is taking more time than next tick time.|