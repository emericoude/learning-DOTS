using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class SetupSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _personPrefab;
    [SerializeField] private int _gridSize;
    [SerializeField] private int _spread;

    [SerializeField] private Vector2 _speedRange = new Vector2(1, 5);
    [SerializeField] private Vector2 _lifeTimeRange = new Vector2(5, 15);

    [SerializeField] private bool _withDOTS = true;

    private BlobAssetStore _blob; //Blob has to do with memory allocation

    private void Start()
    {
        if (_withDOTS)
        {
            InstantiateWithDOTS();
        }
        else
        {
            InstantiateWithoutDOTS();
        }

    }

    private void OnDestroy()
    {
        _blob.Dispose(); //dispose of the allocated blob memory
    }

    private void InstantiateWithDOTS()
    {
        _blob = new BlobAssetStore(); //store memory

        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blob); //_blob uses to stored memory
        Entity personEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(_personPrefab, settings); //convert prefab to a "DOTS-prefab"
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        float3 destination = _gridSize / 2 * _spread;

        for (int x = 0; x < _gridSize; x++)
        {
            for (int z = 0; z < _gridSize; z++)
            {
                Entity instance = entityManager.Instantiate(personEntity); //Create an instance
                float3 position = new float3(x * _spread, 0, z * _spread); //Create a new position
                float speed = UnityEngine.Random.Range(_speedRange.x, _speedRange.y);
                float lifeTime = UnityEngine.Random.Range(_lifeTimeRange.x, _lifeTimeRange.y);

                //Set the instance's position
                entityManager.SetComponentData(instance, new Translation { Value = position });
                entityManager.SetComponentData(instance, new Rotation { Value = quaternion.identity });
                entityManager.SetComponentData(instance, new Destination { Value = destination });
                entityManager.SetComponentData(instance, new Speed { Value = speed });
                entityManager.SetComponentData(instance, new Lifetime { Value = lifeTime });
            }
        }
    }

    private void InstantiateWithoutDOTS()
    {
        for (int x = 0; x < _gridSize; x++)
        {
            for (int z = 0; z < _gridSize; z++)
            {
                Vector3 position = new Vector3(x * _spread, 0, z * _spread); //Create a new position
                GameObject instance = Instantiate(_personPrefab, position, Quaternion.identity);
            }
        }
    }
}
