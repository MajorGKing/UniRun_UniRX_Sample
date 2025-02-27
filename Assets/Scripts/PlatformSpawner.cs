﻿using UnityEngine;
using UniRx;
using UniRx.Triggers;

// 발판을 생성하고 주기적으로 재배치하는 스크립트
public class PlatformSpawner : MonoBehaviour {
    public GameObject platformPrefab; // 생성할 발판의 원본 프리팹
    public int count = 3; // 생성할 발판의 개수

    public float timeBetSpawnMin = 1.25f; // 다음 배치까지의 시간 간격 최솟값
    public float timeBetSpawnMax = 2.25f; // 다음 배치까지의 시간 간격 최댓값
    private float timeBetSpawn; // 다음 배치까지의 시간 간격

    public float yMin = -3.5f; // 배치할 위치의 최소 y값
    public float yMax = 1.5f; // 배치할 위치의 최대 y값
    private float xPos = 20f; // 배치할 위치의 x 값

    private GameObject[] platforms; // 미리 생성한 발판들
    private int currentIndex = 0; // 사용할 현재 순번의 발판

    // 초반에 생성된 발판들을 화면 밖에 숨겨둘 위치
    private Vector2 poolPosition = new Vector2(0, -25);
    private float lastSpawnTime; // 마지막 배치 시점

    // 변수들을 초기화하고 사용할 발판들을 미리 생성
    void Start() {
        // count 만큼의 공간을 가지는 새로운 발판 배열 생성
        platforms = new GameObject[count];

        // count 만큼 루프하면서 발판을 생성
        for (int i = 0; i < count; i++)
        {
            // platformPrefab을 원본으로 새 발판을 poolPosition 위치에 복제 생성
            // 생성된 발판을 platform 배열에 할당
            platforms[i] = Instantiate(platformPrefab, poolPosition, Quaternion.identity);
        }

        // 마지막 배치 시점 초기화
        lastSpawnTime = 0f;
        // 다음번 배치 까지의 시간 간격을 0으로 초기화
        timeBetSpawn = 0f;

        this.UpdateAsObservable()
        .Where(_ => Time.time >= lastSpawnTime + timeBetSpawn)
        .Take(1)
        .RepeatUntilDestroy(this)
        .Subscribe(_ =>
        {
            lastSpawnTime = Time.time;
            timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);

            float yPos = Random.Range(yMin, yMax);

            platforms[currentIndex].SetActive(false);
            platforms[currentIndex].SetActive(true);

            platforms[currentIndex].transform.position = new Vector2(xPos, yPos);
            
            currentIndex++;

            if (currentIndex >= count)
            {
                currentIndex = 0;
            }
        });
    }
}