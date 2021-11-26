﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneratorController : MonoBehaviour
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
    // list untuk pooling object
    private Dictionary<string,List<GameObject>> pool;
    // Start is called before the first frame update
    void Start()
    {
        pool = new Dictionary<string, List<GameObject>>();
        spawnedTerrain = new List<GameObject>();
        lastGeneratedPositionX = GetHorizontalPositionStart();
        lastRemovedPositionX = lastGeneratedPositionX - terrainTemplateWidth;
        foreach(TerrainTemplateController terrain in earlyTerrainTemplates){
            GenerateTerrain(lastGeneratedPositionX,terrain);
            lastGeneratedPositionX += terrainTemplateWidth;
        }

        while(lastGeneratedPositionX < GetHorizontalPositionEnd()){
            GenerateTerrain(lastGeneratedPositionX);
            lastGeneratedPositionX += terrainTemplateWidth;
        }
    }

    // Update is called once per frame
    void Update()
    {
        while(lastGeneratedPositionX < GetHorizontalPositionEnd()){
            GenerateTerrain(lastGeneratedPositionX);
            lastGeneratedPositionX += terrainTemplateWidth;
        }
        // Debug.Log(GetHorizontalPositionStart());
        while(lastRemovedPositionX + terrainTemplateWidth < GetHorizontalPositionStart()){
            // Debug.Log("OYYYYYYYYYYY");
            lastRemovedPositionX+=terrainTemplateWidth;
            RemoveTerrain(lastRemovedPositionX);
        }
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
    private float GetHorizontalPositionStart()
    {
        return gameCamera.ViewportToWorldPoint(new Vector2(0f, 0f)).x + areaStartOffset;
    }

    private float GetHorizontalPositionEnd()
    {
        return gameCamera.ViewportToWorldPoint(new Vector2(1f, 0f)).x + areaEndOffset;
    }
    private void GenerateTerrain(float posx, TerrainTemplateController forceterrain = null){
        GameObject terrainBaru = null;
        if(forceterrain){
            terrainBaru = GenerateFromPool(forceterrain.gameObject,transform);
        }
        else{
            terrainBaru = GenerateFromPool(terrainTemplates[Random.Range(0,terrainTemplates.Count)].gameObject,transform);
        }
        terrainBaru.transform.position = new Vector2(posx,-4.245113f);
        spawnedTerrain.Add(terrainBaru);
    }
    private GameObject GenerateFromPool(GameObject item, Transform parent){
        if(pool.ContainsKey(item.name)){
            if(pool[item.name].Count>0){
                // Jika item ada di dalam pool maka aktifkan
                GameObject newItemFromPool = pool[item.name][0];
                pool[item.name].Remove(newItemFromPool);
                newItemFromPool.SetActive(true);
                return newItemFromPool;
            }
        }
        else{
            pool.Add(item.name,new List<GameObject>());
        }
        GameObject newitem = Instantiate(item,parent);
        newitem.name = item.name;
        return newitem;
    }
    private void RemoveTerrain(float posX)
    {
        GameObject terrainToRemove = null;
        // cari terrain
        foreach (GameObject item in spawnedTerrain)
        {
            if (item.transform.position.x == posX)
            {
                Debug.Log("TESTO");
                terrainToRemove = item;
                break;
            }
        }
        // kalau sudah ketemu
        if (terrainToRemove != null)
        {
            spawnedTerrain.Remove(terrainToRemove);
            ReturnToPool(terrainToRemove);
        }
    }
    private void ReturnToPool(GameObject item){
        if(!pool.ContainsKey(item.name)){
            Debug.Log("Invalid Pool!");
        }
        pool[item.name].Add(item);
        item.SetActive(false);
    }
}
