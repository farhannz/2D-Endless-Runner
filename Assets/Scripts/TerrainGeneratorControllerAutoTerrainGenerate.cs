using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneratorControllerAutoTerrainGenerate : MonoBehaviour
{
    [Header("Templates")]
    public List<TerrainTemplateController> terrainTemplates;
    public float terrainTemplateWidth;

    [Header("Generator Area")]
    public Camera gameCamera;
    public float areaStartOffset;
    public float areaEndOffset;
    private const float debugLineHeight = 10.0f;

    private List<GameObject> spawnedTerrain;
    private float lastGeneratedPositionX;
    private float lastRemovedPositionX;
    [Header("Force Early Template")]
    public List<TerrainTemplateController> earlyTerrainTemplates;

    private void Start()
    {
        spawnedTerrain = new List<GameObject>();

        lastGeneratedPositionX = GetHorizontalPositionStart();

        foreach (TerrainTemplateController terrain in earlyTerrainTemplates)
        {
            GenerateTerrain(lastGeneratedPositionX, terrain);
            lastGeneratedPositionX += terrainTemplateWidth;
        }

        while (lastGeneratedPositionX < GetHorizontalPositionEnd())
        {
            GenerateTerrain(lastGeneratedPositionX);
            lastGeneratedPositionX += terrainTemplateWidth;
        }
    }
    // // Start is called before the first frame update
    // void Start()
    // {
    //     spawnedTerrain = new List<GameObject>();
    //     lastGeneratedPositionX = GetHorizontalPositionStart();
    //     while(lastGeneratedPositionX < GetHorizontalPositionEnd()){
    //         GenerateTerrain(lastGeneratedPositionX);
    //         lastGeneratedPositionX+= terrainTemplateWidth;
    //     }
    // }

    // Update is called once per frame
    void Update()
    {
        while(lastGeneratedPositionX < GetHorizontalPositionEnd()){
            GenerateTerrain(lastGeneratedPositionX);
            lastGeneratedPositionX += terrainTemplateWidth;
        }
        while (lastRemovedPositionX + terrainTemplateWidth < GetHorizontalPositionStart())
        {
            lastRemovedPositionX += terrainTemplateWidth;
            RemoveTerrain(lastRemovedPositionX);
        }
    }
    private float GetHorizontalPositionStart()
    {
        return gameCamera.ViewportToWorldPoint(new Vector2(0f, 0f)).x + areaStartOffset;
    }

    private float GetHorizontalPositionEnd()
    {
        return gameCamera.ViewportToWorldPoint(new Vector2(1f, 0f)).x + areaEndOffset;
    }
    private void OnDrawGizmos()
    {
        // ambil posisi
        Vector3 areaStartPosition = transform.position;
        Vector3 areaEndPosition = transform.position;

        // Set posisi offset
        areaStartPosition.x = GetHorizontalPositionStart();
        areaEndPosition.x = GetHorizontalPositionEnd();

        // Tampilkan gizmos
        Debug.DrawLine(areaStartPosition + Vector3.up * debugLineHeight / 2, areaStartPosition + Vector3.down * debugLineHeight / 2, Color.red);
        Debug.DrawLine(areaEndPosition + Vector3.up * debugLineHeight / 2, areaEndPosition + Vector3.down * debugLineHeight / 2, Color.red);
    }

    public void GenerateTerrain(float posX, TerrainTemplateController forceterrain = null){
        // Instantiate terrain baru
        GameObject newTerrain = Instantiate(terrainTemplates[Random.Range(0,terrainTemplates.Count)].gameObject,transform);
        // Pindahkan ke posisi PosX
        newTerrain.transform.position = new Vector2(posX,-4.3f);
        // Masukkan ke dalam list
        spawnedTerrain.Add(newTerrain);
    }
    
    private void RemoveTerrain(float posX)
    {
        GameObject terrainToRemove = null;
        // cari terrain
        foreach (GameObject item in spawnedTerrain)
        {
            if (item.transform.position.x == posX)
            {
                terrainToRemove = item;
                break;
            }
        }
        // kalau sudah ketemu
        if (terrainToRemove != null)
        {
            spawnedTerrain.Remove(terrainToRemove);
            Destroy(terrainToRemove);
        }
    }
}
