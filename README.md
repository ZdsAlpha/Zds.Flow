
# Zds.Flow

A library to control the flow of program is a completely different way. This library can be used as partial replacement of old Asynchronous system in .Net.

You can design applications with massive parallelism without dealing with thread safety. You can create applications completely detached from UI. You can easily and safely interact with it.
  
There are four major namespaces in library:
  
- Updaters (Objects that create threads, Acts as ThreadPool)
- Updatables (Objects that use threads created by Updaters)
- Collections (Contains different Thread Safe/Unsafe collections)
- Machinery (Used to design a graph that controls the flow of objects)
  
# Things to Remember
  
1. Every objects that implements `IStartStopable` interface will not work unless you `Start` it. Every Updater/Updatable object implements `IStartStopable` interface. So you have to start them manually. You can stop them by calling `Stop`. You can check object status by using `IStartStopable.IsRunning` property.
2. An `Updatable` object will never work without an `Updater`. By default there is a global `Updater` that is assigned to `Updatable` if `Updater` is not given. You can assign an `Updater` to an `Updateable` by setting `Updatable.Updater` or calling `Updater.Add()`. You can get all `Updatables` assigned to an `Updater` by using `Updater.Targets` property.
3. A thread safe collection has prefix `Safe` in its name. So be careful when using collections in Asynchronous objects or in a `Machinery`.
4. Objects that have managed/unmanaged resources implement `IDisposable`. You can check if an object is disposed by calling `*.IsDisposed`.
5. Objects that throw exceptions implement `IThrowsException` interface. These objects have event `OnException`. You can use this event to keep track of exceptions. You can even create an `ExceptionHandler` and add your `IThrowsException` object to it.
  
# Updatables

Updatables are objects that implement `IUpdatable`. These objects have method `Update()` that is recursively called by an `Updater` assigned to it. It is not fixed how fast or delayed the method will be called. So, it should be handled by `Updatable`. All `Updatables` implement `IDisposable`  `IThrowsException` and `IStartStopable`.
  
Every object in `Updatable` namespace has at least one overridable object that can be overridden to define the behaviour of object. You can also do it by passing pointer of a method of same signature through constructor to define behaviour. You can also use associated event. So, inheritance is not MUST.
  
Here is the list of all `Updatables` and their functionalities.
  
|Updatables|Functionality|
|--|--|
|Updatable|Its method `OnUpdated` is called recursively. `OnUpdated` is an overridable method. You can inherit the object to define what to do. You can also define what to do by passing pointer to method matching the signature of `OnUpdated` or using `PreUpdate` or `PostUpdate` events. `PreUpdate` is called before `OnUpdated` is called and `PostUpdate` is called after `OnUpdated` is called.|
|SyncObject|Inherits `Updatable` and overrides method `OnUpdated`. Its method `SyncUpdate` is called recursively such that only one thread is in `SyncUpdate` method at a time. You can get lock by using `Lock` property.|
|AsyncObject|Inherits `Updatable` and overrides method `OnUpdated`. Its method `AsyncUpdate` is recursively called by unspecific number of threads. That number depends on multiple factors e.g `Updater`. The number of threads will not exceed `MaxThread` property of `AsyncObject`. You can get total threads currently stuck in `AsyncUpdate` by using `ActiveThreads` property.|
|SyncTimer|Inherits `SyncObject` and overrides method `SyncUpdate`. Its method `Tick` is called periodically after `SyncTimer.Delay` time such that only one thread is in `Tick` method. Next tick time does not depend on previous tick time but you can make timer tolerant by enabling `SyncTimer.IsTolerant`. In each tick timer calculates error (difference between actual tick time and expected tick time), if `SyncTimer.ErrorCorrection` is enabled this time will be subtracted from next tick time of timer. You can force a `Tick` at any instance by calling `SyncTimer.TickNow()`.|
|AsyncTimer|Inherits `AsyncObject` and overrides method `AsyncUpdate` method. Its method `Tick` is called periodically after `AsyncTimer.Delay` time by unspecific number of threads. Next tick time does not depend on previous tick time but you can make timer tolerant by enabling `AsyncTimer.IsTolerant`. In each tick timer calculates error (difference between actual tick time and expected tick time), if `AsyncTimer.ErrorCorrection` is enabled this time will be subtracted from next tick time of timer. You can force a `Tick` at any instance by calling `AsyncTimer.TickNow()`.|

# Machinery

All objects in `Machinery` namespace implement `ISink`. A `Sink` is like a function that takes an input and behaves on the basis of input. `ISink<T>` has method `Send(T obj)` that lets any code send an object to sink. A `Sink` can also send object to other `Sink` and can make an execution graph. All objects in `Machinery` inherit classes from `Updatables`. They combine functionality of `Sink` and `Updatable`. Updatable behaviour allows it to execute parallel, (independent of sender), while `Sink` allows functionality to receive objects or batch of objects. 