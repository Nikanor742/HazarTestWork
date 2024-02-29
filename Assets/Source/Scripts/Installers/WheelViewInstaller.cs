using UnityEngine;
using Zenject;

public class WheelViewInstaller : MonoInstaller
{
    [SerializeField] private WheelView _wheelPrefab;
    [SerializeField] private Canvas _canvas;

    public override void InstallBindings()
    {
        var wheelInstance = Container.InstantiatePrefabForComponent<WheelView>(_wheelPrefab, _canvas.transform);
        Container.Bind<WheelView>().FromInstance(wheelInstance);
    }
}