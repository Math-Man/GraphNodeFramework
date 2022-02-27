using System;
using System.Collections;
using Framework.Scripts.Model;
using Framework.Scripts.Objects.GraphNodeComponents;
using UnityEngine;

namespace Sample.Colony_Sample_Project.Scripts
{
    public class CoreSupplier : MonoBehaviour
    {
        [SerializeField] private float period = 0.5f;
        [SerializeField] private float amount = 1f;
        [SerializeField] private float maxInternalStorage = 10f;
        [SerializeField] private float sendAmount = 1f;
        
        private float stored = 0f;
        private float inTransit = 0f;
        
        private Coroutine generatorRoutine;
        private PacketSpawnerComponent spawner;
        private BaseCore core;
        
        private void Start()
        {
            spawner = GetComponent<PacketSpawnerComponent>();
            generatorRoutine = StartCoroutine(Generate());
            core = FindObjectOfType<BaseCore>();
        }


        private void Update()
        {
            if (maxInternalStorage > sendAmount)
            {
                var reqData = new PacketRequestData();
                reqData.InitCallbacks(packetSent,packetFail, packetSuccess);
                spawner.addPacketRequest(core.node, reqData);
            }
        }

        private void OnDestroy()
        {
            if(generatorRoutine != null)
                StopCoroutine(generatorRoutine);
        }

        private IEnumerator Generate()
        {
            while (true)
            {
                if(stored < maxInternalStorage)
                    stored += amount;
                yield return new WaitForSeconds(period);
            }
        }



        private void packetSent(PacketRequestResultData result)
        {
            stored -= amount;
            inTransit += amount;
        }

        private void packetSuccess(PacketRequestResultData result)
        {
            inTransit -= amount;
            core.storedEnergy += amount;
        }

        private void packetFail(PacketRequestResultData result)
        {
            inTransit -= amount;
            //its lost forever :(
        }

    }
}