using System.Collections; // <-- DIPERLUKAN UNTUK COROUTINE
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps; // <-- DIPERLUKAN UNTUK TILEMAP

public class ObjectSpawner : MonoBehaviour
{
    public enum ObjectType { Enemy }

    [Header("Referensi")]
    public Tilemap tilemap;
    public GameObject[] objectPrefabs;

    [Header("Pengaturan Spawning")]
    public float enemyProbability = 1.0f; // Kita buat 100% untuk tes dulu
    public int maxObjects = 5;
    public float spawnInterval = 0.5f;

    private List<Vector3> validSpawnPositions = new List<Vector3>();
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private bool isSpawning = false;

    void Start()
    {
        GatherValidPositions();
        //GameController.OnReset += LevelChange;
    }

    void Update()
    {
        // Cek di Update apakah perlu memulai proses spawning
        if (!isSpawning && ActiveObjectCount() < maxObjects)
        {
            StartCoroutine(SpawnObjectsIfNeeded());
        }
    }

    private int ActiveObjectCount()
    {
        spawnedObjects.RemoveAll(item => item == null);
        return spawnedObjects.Count;
    }

    private IEnumerator SpawnObjectsIfNeeded()
    {
        isSpawning = true;

        // Loop untuk spawn objek sampai mencapai batas maksimum
        while (ActiveObjectCount() < maxObjects)
        {
            SpawnObject();
            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false;
    }

    private bool PositionHasObject(Vector3 positionToCheck)
    {
        // Pengecekan disederhanakan
        return spawnedObjects.Any(obj => obj != null && Vector3.Distance(obj.transform.position, positionToCheck) < 1.0f);
    }

    private ObjectType RandomObjectType()
    {
        // Untuk saat ini, kita hanya punya Enemy, jadi langsung kembalikan Enemy
        // Jika nanti ada tipe lain, logika if/else bisa ditambahkan di sini
        return ObjectType.Enemy;
    }

    private void SpawnObject()
    {
        if (validSpawnPositions.Count == 0) return;

        // Ambil posisi acak dari daftar yang valid
        int randomIndex = Random.Range(0, validSpawnPositions.Count);
        Vector3 spawnPosition = validSpawnPositions[randomIndex];

        // Cek jika sudah ada objek di dekatnya sebelum spawn
        if (!PositionHasObject(spawnPosition))
        {
            ObjectType objectType = RandomObjectType();
            // Perbaiki penulisan Quaternion.identity
            GameObject newObject = Instantiate(objectPrefabs[(int)objectType], spawnPosition, Quaternion.identity);
            spawnedObjects.Add(newObject);
        }
    }
    
    // Fungsi DestroyObjectAfterTime tidak kita gunakan di versi ini, bisa ditambahkan nanti jika perlu
    // private IEnumerator DestroyObjectAfterTime(GameObject gameObject, float time) { ... }

    private void GatherValidPositions()
    {
        validSpawnPositions.Clear();
        if (tilemap == null)
        {
            Debug.LogError("Tilemap belum di-assign di ObjectSpawner!");
            return;
        }

        BoundsInt bounds = tilemap.cellBounds;
        
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int localPlace = new Vector3Int(x, y, 0);
                Vector3Int placeAbove = new Vector3Int(x, y + 1, 0);

                // Cek jika tile di posisi ini ada, TAPI tile di atasnya KOSONG
                // Ini memastikan kita hanya spawn di permukaan platform, bukan di dalam atau di udara
                if (tilemap.HasTile(localPlace) && !tilemap.HasTile(placeAbove))
                {
                    Vector3 worldPosition = tilemap.GetCellCenterWorld(localPlace);
                    // Naikkan sedikit posisi spawn agar objek tidak masuk ke dalam tanah
                    validSpawnPositions.Add(worldPosition + new Vector3(0, 1f, 0));
                }
            }
        }
    }
}