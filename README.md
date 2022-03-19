# Simple Object Pool

A simple singleton object pool for Unity.

## Features
- Any GameObject prefab can be pooled, don't have to attach or inherit some particular class.
- You can directly create and edit pools via the inspector, on a single manager game object.
- Pools can dynamically expand if they run out of avaliable pooled objects.
- You don't have to explicitly return a pooled object back to its pool -- it will be avaliable in the pool as long as it's not active in hierarchy.

## Limitations
- Only supports GameObject pooling, 
- Pools might expand unnecessarily if not all of its pooled objects are returned in chronological order (see [How It Works](#how-it-works))

  (This won't really be a problem for stuff like bullets and hit VFX that are typically in use for a very short amount of time)

# How It Works
`ObjectPool` is a singleton class that stores an array of `Pool`, each maintains a pool for a particular prefab in `LinkedList<GameObject>`. These pools can be configured on the inspector, and are initialized on `Awake()`.

Each `Pool` can be identified by a string *tag*.

1. When `ObjectPool.instance.SpawnFromPool(tag, ...)` is called from some script, the *tag* is passed to `Dictionary<tag, Pool>` to find the corresponding pool.

2. The pool checks whether the first element of `LinkedList<GameObject>` is active in hierarchy. If true, it assumes all pooled objects in this pool are in use, then instantiates a specified amount of prefabs to expand the pool. Each of these fresh instances will be added to the beginning of `LinkedList<GameObject>` so they can be immediately used next.

3. The first element of `LinkedList<GameObject>` is removed, and added as the last element of `LinkedList<GameObject>`. This is the pooled object returned by `SpawnFromPool(tag, ...)`.



# Usage
## Setup
1. Create a GameObject, attach the ObjectPool component to it.
2. Setup pools.

![example](https://i.ibb.co/1XG4Wgx/simple-object-pool-1.jpg)

*tag*: String tag for identifying this pool.

*prefab*: Prefab to pool.

*size*: Initial size of the pool.

*refill*: Amount of new pooled objects to instantiate if all pooled objects are in use.

*maxSize*: Maximum size of the pool.

## Spawning a Pooled Object

`ObjectPool` has method
```
 public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, Transform parent = null)
```

 `SpawnFromPool` does not activate the pooled object.

You can get a pooled object by `SpawnFromPool`, make sure the things on it you need are set up, then activate it `GameObject.SetActive(true)`

Example:
```
Gameobject _gameObject = ObjectPool.instance.SpawnFromPool(string tag, transform.position, Quaternion.identity, parent: null)

// Initialize the pooled object here.

_gameObject.SetActive(true);
```

---

Examples (directly getting a component):
```
SomeClass _component = ObjectPool.instance.SpawnFromPool(tag, transform.position, Quaternion.identity)
.GetComponent<SomeClass>();

// Initialize component here.

_component.gameObject.SetActive(true);
```
or

```
If (ObjectPool.instance.SpawnFromPool(tag, transform.position, Quaternion.identity)
.TryGetComponent<SomeClass>(out SomeClass _component))
{
    // Initialize component here.

    _component.gameObject.SetActive(true);
}
```
If the pooled object has a script component, initialization can be put in its `OnEnable()`.

---

## Returning a Pooled Object
Simply
```
gameObject.SetActive(false);
```
You could attach a [Timer] on the pooled object, or let whatever script on it handle this.

Stuff you want to do when returning a pooled object can be done in its `OnDisable()`.

[//]: # (These are reference links used in the body of this note and get stripped out when the markdown processor does its job. There is no need to format nicely because it shouldn't be seen. Thanks SO - http://stackoverflow.com/questions/4823468/store-comments-in-markdown-syntax)

[Timer]: <https://www.youtube.com/watch?v=pRjTM3pzqDw>
