# Zds.Flow
A library to control the flow of program is a completely different way. It puts life to the program. This library can be used as partial replacement of old Asynchronous system in .Net.
You can design applications with massive parallelism without dealing with thread safety. You can create applications completely detached from UI. You can easily and safely interact with it.

There are four major namespaces in library:

 - Updaters  (Objects that create threads, Acts as ThreadPool)
 - Updatables  (Objects that use threads created by Updaters)
 - Collections  (Contains different Thread Safe/Unsafe collections)
 - Machinery  (Used to design a graph that controls the flow of objects)

Other namespaces:

 - DelayHandling  (Objects mostly used by Updates to block one thread to allow the flow of other threads)
 - ExceptionHandling  (Objects for handling exceptions)
 - Stopwatch  (Objects for tracking time)
 - Interfaces  (Interfaces that do not fit in any category)

# Things to Remember

 1. Every objects that implements `IStartStopable` interface will not work unless you `Start` it. Every Updater/Updatable object implements `IStartStopable` interface. So you have to start them manually. You can stop them by calling `Stop`. You can check object status by using `IStartStopable.IsRunning` property.
 2. An `Updatable` object will never work without an `Updater`. By default there is a global `Updater` that is assigned to `Updatable` if `Updater` is not given. You can assign an `Updater` to an `Updateable` by setting `Updatable.Updater` or calling `Updater.Add()`. You can get all `Updatables` assigned to an `Updater` by using `Updater.Targets` property.
 3. A thread safe collection has prefix `Safe` in its name. So be careful when using collections using by Asynchronous objects or in a `Machinery`.
 4. Objects that have managed/unmanaged resources implement `IDestroyable`. These objects have method `Destroy()`. When you call it, the object will release its resources and will stop functioning. You can check whatever an object is destroyed or not by using `IDestroyable.IsDestroyed` property.
 5.  Objects that throw exceptions implement `IThrowsException` interface. These objects have event `OnException`. You can use this event to keep track of exceptions. You can even create an `ExceptionHandler` and add your `IThrowsException` object to it.

# Updatables
Updatables are objects that implement `IUpdatable`. These objects have method `Update()` that is recursively called by an `Updater` assigned to it. It is not fixed how fast or delayed the method will be called. So, it should be handled by `Updatable`. All `Updatables` implement `IDestroyable` `IThrowsException` and `IStartStopable`. 

Every object in `Updatable` namespace has at least on overridable object that can be overridden to define the behaviour of object. You can also do it by passing pointer of a method of same signature through constructor to define behaviour. You can also use associated event. So, inheritance is not MUST.

Here is the list of all `Updatables` and their functionalities.

|Updatables|Functionality|
|--|--|
|Updatable|Its method `OnUpdated` is called recursively. `OnUpdated` is an overridable method. You can inherit the object to define what to do. You can also define what to do by passing pointer to method matching the signature of `OnUpdated` or using `PreUpdate` or `PostUpdate` events. `PreUpdate` is called before `OnUpdated` is called and `PostUpdate` is called after `OnUpdated` is called.|
|SyncObject|Inherits `Updatable` and overrides method `OnUpdated`. Its Method `SyncUpdate` is called recursively such that only one thread is in `SyncUpdate` method at a time. You can get lock by using `Lock` property.|
|AsyncObject|Inherits `Updatable` and overrides method `OnUpdated`. Its method `AsyncUpdate` is recursively called unspecific number of threads. That number depends on multiple factors e.g `Updater`. The number of threads will not exceed `MaxThread` property of `AsyncObject`. You can get total threads currently stuck in `AsyncUpdate` by using `ActiveThreads` property|
