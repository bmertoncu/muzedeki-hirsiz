using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VisionCone : MonoBehaviour
{
    [Header("2D Koni / Görüş Alanı Ayarları")]
    [Tooltip("Koninin açısı (derece cinsinden FOV).")]
    [Range(15f, 90f)]
    public float fovAngle = 38f;

    [Tooltip("Koninin menzili / uzunluğu.")]
    [Range(3f, 25f)]
    public float distance = 14f;

    [Tooltip("Koninin yay segmenti sayısı. 1 = Üçgen, 16 = Pürüzsüz kavisli daire dilimi.")]
    [Range(1, 32)]
    public int segments = 16;

    [Tooltip("2D koninin düzlemsel kalınlığı (fizik çarpışmaları ve çift taraflı görünüm için).")]
    public float thickness = 0.08f;

    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private Mesh coneMesh;

    private float lastFov = -1f;
    private float lastDist = -1f;
    private int lastSeg = -1;
    private float lastThick = -1f;

    private void Awake()
    {
        if (Application.isPlaying)
        {
            // Hareket eden collider'ın statik fizik ağacını her kare yeniden hesaplatmaması için Kinematic Rigidbody ekle
            if (!TryGetComponent<Rigidbody>(out var rb))
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        GenerateCone(false);
    }

    private void OnEnable()
    {
        GenerateCone(false);
    }

    private void OnValidate()
    {
        GenerateCone(false);
    }

    [ContextMenu("Koniyi Yeniden Oluştur")]
    public void ForceRegenerateCone()
    {
        GenerateCone(true);
    }

    public void GenerateCone(bool force = false)
    {
        if (meshFilter == null) meshFilter = GetComponent<MeshFilter>();
        if (meshCollider == null) meshCollider = GetComponent<MeshCollider>();

        // Eski BoxCollider varsa devre dışı bırak
        if (TryGetComponent<BoxCollider>(out var box) && box.enabled)
        {
            box.enabled = false;
        }

        int seg = Mathf.Max(1, segments);

        // Parametreler değişmediyse ve mesh zaten hazırsa tekrar hesaplama (Performans optimizasyonu)
        if (!force && coneMesh != null && meshFilter != null && meshFilter.sharedMesh == coneMesh &&
            Mathf.Approximately(lastFov, fovAngle) &&
            Mathf.Approximately(lastDist, distance) &&
            lastSeg == seg &&
            Mathf.Approximately(lastThick, thickness))
        {
            return;
        }

        lastFov = fovAngle;
        lastDist = distance;
        lastSeg = seg;
        lastThick = thickness;

        if (coneMesh == null)
        {
            coneMesh = new Mesh();
            coneMesh.name = "VisionCone2D_ProceduralMesh";
        }
        else
        {
            coneMesh.Clear();
        }
        float halfAngle = fovAngle * 0.5f * Mathf.Deg2Rad;
        float halfThick = thickness * 0.5f;

        // Tepe noktası: (0, 0, 0)
        // Üst yüzey için: Tepe (0), Arc noktaları 1..seg+1
        // Alt yüzey için: Tepe (seg+2), Arc noktaları seg+3..2*seg+3
        int countPerSide = seg + 2;
        Vector3[] vertices = new Vector3[countPerSide * 2];
        Vector2[] uvs = new Vector2[vertices.Length];

        // 1. Üst tepe noktası (Apex Top)
        vertices[0] = new Vector3(0f, halfThick, 0f);
        uvs[0] = new Vector2(0.5f, 0f);

        // 2. Üst yay noktaları (Top Arc)
        for (int i = 0; i <= seg; i++)
        {
            float t = (float)i / seg;
            float angle = Mathf.Lerp(-halfAngle, halfAngle, t);
            float x = distance * Mathf.Sin(angle);
            float z = distance * Mathf.Cos(angle);
            vertices[1 + i] = new Vector3(x, halfThick, z);
            uvs[1 + i] = new Vector2(t, 1f);
        }

        // 3. Alt tepe noktası (Apex Bottom)
        int botApexIndex = countPerSide;
        vertices[botApexIndex] = new Vector3(0f, -halfThick, 0f);
        uvs[botApexIndex] = new Vector2(0.5f, 0f);

        // 4. Alt yay noktaları (Bottom Arc)
        for (int i = 0; i <= seg; i++)
        {
            Vector3 topV = vertices[1 + i];
            vertices[botApexIndex + 1 + i] = new Vector3(topV.x, -halfThick, topV.z);
            uvs[botApexIndex + 1 + i] = new Vector2((float)i / seg, 1f);
        }

        System.Collections.Generic.List<int> tris = new System.Collections.Generic.List<int>();

        // Üst Yüzey (Triangle Fan, Normal yukarı +Y)
        for (int i = 0; i < seg; i++)
        {
            tris.Add(0);
            tris.Add(1 + i);
            tris.Add(1 + i + 1);
        }

        // Alt Yüzey (Triangle Fan, Normal aşağı -Y)
        for (int i = 0; i < seg; i++)
        {
            tris.Add(botApexIndex);
            tris.Add(botApexIndex + 1 + i + 1);
            tris.Add(botApexIndex + 1 + i);
        }

        // Sol Kenar (Apex -> İlk Yay Noktası)
        // 0 (top apex), 1 (top left), botApexIndex+1 (bot left), botApexIndex (bot apex)
        tris.Add(0);
        tris.Add(botApexIndex);
        tris.Add(botApexIndex + 1);

        tris.Add(0);
        tris.Add(botApexIndex + 1);
        tris.Add(1);

        // Sağ Kenar (Apex -> Son Yay Noktası)
        int topRight = 1 + seg;
        int botRight = botApexIndex + 1 + seg;
        tris.Add(topRight);
        tris.Add(botRight);
        tris.Add(botApexIndex);

        tris.Add(topRight);
        tris.Add(botApexIndex);
        tris.Add(0);

        // Dış Yay Kenarı (Arc Quads)
        for (int i = 0; i < seg; i++)
        {
            int topA = 1 + i;
            int topB = 1 + i + 1;
            int botA = botApexIndex + 1 + i;
            int botB = botApexIndex + 1 + i + 1;

            tris.Add(topA);
            tris.Add(topB);
            tris.Add(botB);

            tris.Add(topA);
            tris.Add(botB);
            tris.Add(botA);
        }

        coneMesh.vertices = vertices;
        coneMesh.uv = uvs;
        coneMesh.triangles = tris.ToArray();
        coneMesh.RecalculateNormals();
        coneMesh.RecalculateBounds();

        if (meshFilter != null)
        {
            meshFilter.sharedMesh = coneMesh;
        }

        if (meshCollider != null)
        {
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = coneMesh;
            meshCollider.convex = true;
            meshCollider.isTrigger = true;
        }
    }
}
