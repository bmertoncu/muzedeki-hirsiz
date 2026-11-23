using UnityEngine;

public class CargoSpawner : MonoBehaviour
{
    // what to copy, and how long the pad stays empty
    [SerializeField] private GameObject cargoPrefab;
    [SerializeField] private float delay = 4f;

    private float timer;   // seconds left until the next cargo

    void Update()
    {
        // a cargo still waits on this pad? then do nothing
        if (GetComponentInChildren<Cargo>() != null) return;

        // count down: a little less time left every frame
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        // time is up: a copy of the prefab, here, as our child
        Instantiate(cargoPrefab, transform.position,
                    transform.rotation, transform);
        timer = delay;   // the next wait will be this long
    }
}
