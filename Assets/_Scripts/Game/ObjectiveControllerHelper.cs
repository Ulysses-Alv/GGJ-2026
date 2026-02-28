using System;
using UnityEngine;

public class ObjectiveControllerHelper : MonoBehaviour
{
    [SerializeField] private ObjectiveData[] auras;


    public static ObjectiveControllerHelper Instance { get; internal set; }

    [SerializeField] private float TimeToShowAura = 60f;

    ObjectiveType currentType = ObjectiveType.Hallway;

    void Awake()
    {
        Instance = this;
    }
    Coroutine coroutine;
    void Start()
    {
        foreach (var data in auras)
        {
            data.aura.SetActive(false);
        }
     coroutine =   StartCoroutine(StartHelper());
    }

    private System.Collections.IEnumerator StartHelper()
    {
        yield return new WaitForSeconds(TimeToShowAura);
        foreach (var data in auras)
        {
            data.aura.SetActive(data.type == currentType);
        }
    }
    internal void SetObjective(ObjectiveType objectiveType)
    {
        currentType = objectiveType;
        StopCoroutine(coroutine);
        StartCoroutine(StartHelper());
    }

    [Serializable]
    private struct ObjectiveData
    {
        public ObjectiveType type;
        public GameObject aura;
    }
}
internal enum ObjectiveType
{
    Hallway,
    Electricity
}