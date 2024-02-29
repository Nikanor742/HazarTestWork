using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "ScriptableInstaller", menuName = "Installers/ScriptableInstaller")]
public class ScriptableInstaller : ScriptableObjectInstaller<ScriptableInstaller>
{
    public GameConfig gameConfig;
    public WhellConfig wheelConfig;

    public override void InstallBindings()
    {
        Container.BindInstance(gameConfig).AsSingle().NonLazy();
        Container.BindInstance(wheelConfig).AsSingle().NonLazy();
    }
}