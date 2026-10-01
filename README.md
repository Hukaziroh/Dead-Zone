# Dead-Zone

> **몰려오는 적들을 물리치고 끝까지 생존하라!** <br>
> 탑다운 뷰 기반의 2D 서바이벌 슈팅 게임 (뱀파이어 서바이버 장르)

<br>

## 프로젝트 개요
- **개발 기간**: 4개월 (2025.07 ~ 2025.10)
- **개발 인원**: 2명
  - **Main Programmer (본인)**: 핵심 게임 로직(웨이브, 스포너), 플레이어 및 무기 시스템, 최적화 구현
  - **Level & Boss Designer (팀원)**: 4계절(봄/여름/가을/겨울) 맵 디자인, 보스 몬스터 패턴 기획 및 구현
- **사용 기술**: Unity 6, C#

<br>

## 주요 게임플레이
| 몬스터 웨이브 방어 | 보스전 |
|:---:|:---:|
| ![Gameplay](https://via.placeholder.com/400x250?text=Gameplay+GIF+Here) | ![Boss](https://via.placeholder.com/400x250?text=Boss+Fight+GIF+Here) |

<br>

## 핵심 기술 구현 (Technical Features)

### 지형을 고려한 동적 안전 스폰 시스템 (`Spawner.cs`)
- `Physics2D.OverlapPoint`를 활용해 이동 불가 지형(벽, 물) 위에 적이 생성되는 **끼임 버그 원천 차단**
- 프레임 드랍을 막기 위해 **최대 10회 탐색 락(Lock)**을 설정하고, **오브젝트 풀링(Object Pooling)**과 연동하여 메모리 오버헤드 최소화
- 웨이브 난이도(몬스터 종류, 스폰 주기)를 `SpawnData` 직렬화 클래스로 캡슐화하여 밸런싱 편의성 극대화

### 수학적 알고리즘을 적용한 8방향 사격 및 산탄 시스템 (`PlayerAttack.cs`)
- 플레이어 8방향 이동 애니메이션(Blend Tree)에 맞추어 **정확한 총구 오프셋(Muzzle Offset) 동기화**
- 복잡한 분기문(`if-else`) 없이 `((angle + 22.5) / 45) % 8` 수식과 배열 캐싱을 통해 **O(1)의 시간 복잡도로 발사 위치 탐색 최적화**
- **쿼터니언(Quaternion)의 Z축 회전 연산**을 이용해 펠릿(Pellet) 수와 방사각(Spread Angle)을 자유롭게 조절할 수 있는 **무한한 확장성의 산탄(Shotgun) 로직** 구축

### FSM 기반의 적 AI 및 웨이브 관리 체계 (`GameManager.cs` / `Enemy.cs`)
- 적 객체의 상태(추적, 공격, 대기 등)를 코루틴과 FSM 패턴으로 설계하여 효율적인 업데이트 사이클 유지
- 처치 횟수(Kill Count)를 기반으로 자동으로 난이도가 상승하고 다음 웨이브 및 보스전으로 전환되는 메인 루프 아키텍처 구축

<br>

## 리포지토리 주요 구조
```text
Dead-Zone/
 ├── Assets/
 │   ├── Scenes/         # 봄, 여름, 가을, 겨울 테마 맵 및 로비/클리어 씬
 │   ├── Scripts/
 │   │   ├── Manager/    # GameManager, PoolManager, Spawner 등 핵심 제어
 │   │   ├── Player/     # 플레이어 이동, 8방향 사격 로직
 │   │   ├── Gun/        # 샷건, 권총 등 무기 데이터 및 총알 물리 로직
 │   │   └── Enemy/      # 일반 몬스터 및 보스 AI 패턴
 │   └── Prefabs/        # 오브젝트 풀링을 위한 프리팹 데이터
```
