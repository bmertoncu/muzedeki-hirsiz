using UnityEngine;

// adds Cube Courier > Cargo Type to the Create menu
[CreateAssetMenu(menuName = "Cube Courier/Cargo Type")]
public class CargoType : ScriptableObject
{
    [SerializeField] private int points = 1;      // when delivered
    [SerializeField] private float weight = 0f;   // extra mass, kg

    // other scripts may read them, never change them
    public int Points()
    {
        return points;
    }

    public float Weight()
    {
        return weight;
    }
}
