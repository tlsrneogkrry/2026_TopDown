using System.Collections.Generic;
using UnityEngine;

// 인스펙터 창에서 마우스 우클릭 -> Create -> Scriptable Objects -> Stage Wave Data로 파일을 만들 수 있게 해줍니다.
[CreateAssetMenu(fileName = "NewStageWaveData", menuName = "Scriptable Objects/Stage Wave Data")]
public class StageWaveData : ScriptableObject
{
    [System.Serializable]
    public class EnemySpawnInfo
    {
        public string waveName;              // 웨이브 이름 (예: 1분 웨이브, 보스 등장!)
        public List<GameObject> enemyPrefabs;// 이 웨이브에 등장할 적 종류 프리팹들
        public float spawnInterval = 1.0f;   // 몬스터 생성 주기
        public bool isBossWave = false;      // ★ 보스 웨이브인지 판별하는 체크박스
    }

    [Header("시간별 웨이브 설정 (0번 원소 = 1분, 1번 원소 = 2분...)")]
    public List<EnemySpawnInfo> waveList;
}