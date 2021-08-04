## 참고 블로그

**베르의 프로그래밍 노트**<br>

https://wergia.tistory.com/224?category=742236 <br>
https://wergia.tistory.com/225?category=742236 <br>
<br>

## 🔔 Navigation
실생활에서도 많이 들어봤듯이, **네비게이션이란 어떤 한 지점에서 다른 지점으로 이동하는 경로를 알려주는 시스템이다.**<br>
물론 플레이어는 자신의 캐릭터를 잘 조작해서 원하는 지점까지 이동할 수 있겠지만, AI가 조작하는 몬스터나 NPC는 그럴 수 없다.<br>
특히나 아무런 장애물이 없는 일직선의 맵이 아니라 수많은 장애물이 존재하는 맵에서는 더욱이 그럴 것이다.<br>
**네비게이션 시스템은 이러한 환경에서 AI가 최단의 경로를 찾아 목적지에 도달하는 것을 돕는다.** ⭐<br>
<br>

유니티의 네비게이션 시스템은 기본적으로 **NavMesh, NavMeshAgent, NavMeshObstacle** 이 3가지 요소로 이루어진다.<br>
<br>
<br>

## 🔔 NavMesh

![네브메시](https://user-images.githubusercontent.com/43705434/127989353-f9edb219-c87d-4458-a29d-3bdab809eb2e.PNG)<br>
<br>

⭐⭐ **네브메쉬란 3D메시 또는 지형을 분석해 이동할 수 있는 영역, 이동할 수 없는 영역으로 구분한 데이터를 의미한다.**<br>
(**3차원 지형을 2D처럼 간단하게 표현해 이동 가능한 모든 지형을 Cell(삼각형)으로 표시하여 A star 알고리즘을 쉽게 적용할 수 있음.**)<br>
고로 모든 **이동 가능한 삼각형의 집합 데이터를 NavMesh라고 표현할 수 있으며**, 해당 데이터에 A* 알고리즘을 적용해 최단경로를 찾게된다.<br>
(**터레인이나 메시렌더러 컴포넌트**를 가진 게임 오브젝트만 NavMesh 데이터에 포함될 수 있음을 주의.) ⭐⭐<br>

👉 **Bake 탭**<br>
Agent Radius : NavMesh 위에서 움직일 대상의 반지름<br>
Agent Height : NavMesh 위에서 움직일 대상의 키<br>
Max Slope : NavMesh 위에서 움직일 대상이 이동할 수 있는 최대 경사로의 각도<br>
Step Height : NavMesh 위에서 움직일 대상이 가벼운 계단 정도로 여기고 올라갈 수 있는 높이<br>
<br>

👉 **Area 탭**<br>

![네브메시에리아](https://user-images.githubusercontent.com/43705434/127989365-35e05d5c-9dfc-47bc-b1f6-b2805557d1dd.PNG)<br>
<br>

**NavMesh를 사용자가 지정한 여러 Area로 나누고 해당 구역을 지나가는 Cost를 설정할 수 있다.** ⭐<br>
<br>

![워터에리아](https://user-images.githubusercontent.com/43705434/127989976-6d80d19e-83ae-4b34-84da-e6ae518882c3.PNG)<br>
<br>

위 예시처럼 2개의 다리 중 하나의 다리는 Water Area로 설정하고 Cost를 3으로 설정하게 되면,<br>
오브젝트는 **가급적 비용이 덜드는 즉 Water가 없는 구역으로 건너가려고 할 것이다.**<br>

이렇게 Area마다 다른 비용을 책정해서 네브메시를 구성하면 좀 더 **다양한 레벨 디자인을 쉽게 구성** 할 수 있다.<br>

👉 **Agents 탭**<br>
Bake 탭에서 설정하는 옵션들의 **프리셋**을 저장하는 탭으로 미리 에이전트별 프리셋을 저장해두고 사용한다.<br>
ex) **덩치가 큰 에이전트 용 프리셋, 덩치가 작은 에이전트 용 프리셋..**<br>
<br>
<br>

## 🔔 NavMeshAgent
Agent는 네비게이션 메시 위에서 길을 찾아 **움직이게 될 오브젝트 대상**을 의미한다.<br>

* Base Offset : 캐릭터와 에이전트의 위치를 맞추기 위해서 사용되는 프로퍼티<br>

* Speed : 움직이는 속도<br>

* Angular Speed : 회전하는 속도<br>

* Acceleration : 가속도<br>

* Stopping Distance ⭐ : 목표와 얼마만큼의 거리를 두고 멈출 지를 결정한다.<br>

* Auto Breaking : 에이전트가 목적지에 도착하기 직전에 감속을 시작할 것인지를 결정하는 프로퍼티<br>
-> 해당 기능을 off하게 되면 목적지에 도착하고 나서 감속하기 때문에 속도를 주체하지 못하는 현상이 발생<br>

* Obstacle Avoidance : 또 다른 Agent 혹은 NavMeshObstacle을 어떻게 **회피**할 것인지를 결정하는 프로퍼티들<br>
-> 기본 상황에서는 다른 무엇인가 길을 막고 있다면, 돌아갈 길을 찾고 없다면 밀어내서 지나감.<br>

* Radius : 벽과는 관계없이 또 다른 Agent 혹은 NavMeshObstacle과 충돌하는 영역의 두께만 조절된다.<br>

* Height : 에이전트끼리의 높이 충돌을 조절하는 프로퍼티<br>

* Quality : 장애물 회피 품질을 의미한다.<br>
-> HighQuality로 설정하면 정밀한 움직임으로 회피하고 Low Quality로 설정하면 피하는 움직임이 간소화된다.<br>
그리고 양 쪽 다 None으로 설정하면 서로 완전히 무시하고 지나간다.<br>

* Priority ⭐ : 에이전트 간의 우선순위이다. 0이 가장높고 99가 가장 낮다.<br>
-> 우선 순위가 높은 에이전트는 길을 찾을 때 우선 순위가 낮은 에이전트를 고려하지 않고 밀고 지나간다!<br>
-> 우선 순위가 같다면 회피하려는 노력은 하지만 여의치 않을 때는 그냥 밀고 지나가게 된다.<br>
-> 우선 순위가 낮다면 밀어내지 못한다.<br>

* Auto Traverse OffMeshLink ⭐⭐ : 체크를 해제하면 OffMeshLink를 만났을때 자동으로 오브젝트가 이동하지 않고 멈춘다.<br>
-> 코드로 직접 사다리등을 등반하는 움직임에 대해서 컨트로 해줘야 할 때 체크 해제하고 사용하면 된다.<br>

* Auto Repath ⭐ : NavMesh에 변동이 생겼을 때 자동으로 길을 다시 찾을 것인지를 설정하는 프로퍼티<br>
-> ⭐ **하지만 아주 먼거리를 이동할 때 아직 시야에 보이지 않는 중간이 막혀도 경로를 바로 재계산하기 때문에<br>
경로가 막힌 구역까지 도착한 다음 경로를 새로 계산하기를 원한다면 체크를 해제하고 경로가 막힌 구역까지<br>
도착한 다음 경로를 새로 계산하는 기능을 직접 구현해야 한다.** ⭐<br>

* Area Mask : 해당 에이전트가 지나갈 수 있는 Area와 지나갈 수 없는 Area를 설정하는 마스크<br>
<br>
<br>

## 🔔 NavMeshObstacle
해당 컴포넌트는 이름에서도 알 수 있듯이, 장애물 역할을 한다.<br>
⭐ **Navigation Static이 적용된 지형과 NavMeshObstacle은 큰 차이점이 있는데 Navigation Static으로 설정된 벽은 움직일 수 없다는 것이다.<br>
그와 반대로 NavMeshObstacle을 사용하는 장애물은 게임이 플레이되는 도중에 언제든지 움직일 수 있다.** ⭐<br>
그리고 실시간으로 에이전트를 밀어내는 동작도 가능하며 장애물에 밀려난 목적지가 있는 에이전트는 다시 원래 자리로 돌아오려는 움직임으로 보이게 된다.<br>
<br>

* Shape : 장애물의 형태를 결정하는 옵션 Box, Capsule 두 가지 형태를 지원한다.<br>

* Carve : Carve는 "파내다"라는 뜻으로 내비게이션 **메시 영역을 새로 굽지 않아도 실시간으로 "파내서" 에이전트가 지나갈 수 없는 영역으로 만드는 것이다.**<br>
Carve를 켜서 실시간으로 내비게이션 메시를 파내게 하면 큰 장애물도 자연스럽게 회피하게 할 수 있다.<br>
-> **기존에는 장애물 밑 메시 영역이 활성화 되어 있기 때문에 장애물로 인지하지 않는다.**<br>
-> 대신 **실시간 갱신으로 연산 부하가 크기 때문에 아래 3가지 옵션을 통해 최적의 설정값을 찾아야 한다.** ⭐⭐⭐ <br>

* Move Threshold : Carve의 하위 프로퍼티인 Move Threshold는 최소 이동 거리를 뜻한다.<br>
Move Threshold보다 조금 움직인 것은 오브젝트가 움직이지 않은 것으로 간주하여 Carve를 새로 계산하지 않는다.<br>

* Time To Stationary : 게임 오브젝트가 얼마나 정지해있으면 완전히 멈춘 것으로 판정하고,<br>
Carve를 새로 계산해서 내비게이션 메시를 파낼지를 결정하는 값이다.<br>

* Carve Only Stationary는 정지된 상태에서만 내비게이션 메시를 파내도록 할 것인지를 결정하는 프로퍼티이다.<br> 
기본 값은 true로 체크되어 있다. 그래서 장애물이 움직이면 파내진 구멍이 사라졌다가 정지하면 내비게이션 메시가 다시 파내진다.<br>
하지만 Carve Only Stationary를 끄면 장애물이 움직일 때 내비게이션 메시의 파인 구멍이 실시간으로 장애물을 따라 움직인다.<br>
⭐ **자연스러운 움직임을 위해서는 Carve Only Stationary를 끄는게 좋지만, 물체가 움직이는 상황에서 실시간으로 내비게이션 메시에<br>
구멍을 파내는 계산을 계속하는 것은 게임의 성능에 좋지않은 영향을 끼칠 수 있기 때문에 반드시 필요한 경우에만 이 옵션을 끄고 그 외에는 이 옵션을 사용하는게 좋다.**
<br>
<br>
 
유니티 공식 문서에서 언급되듯이 **천천히 움직이는 탱크** 같은 곳에 사용해도 되고 특정한 트리거를 발동하면 떨어져서 **길을 막는 돌무더기** 같은 것에도 사용해도 된다.<br>
<br>
<br>

## 🔔 Off Mesh Link

**Off Mesh Link 사용하기 전 꼭 봐야하는 영상** https://www.youtube.com/watch?v=LezP4pzylIg <br>
<br>

네비게이션은 기본적으로 3D 메시의 정보를 기반으로 네비메시를 생성하기 때문에 서로 분리된 메쉬는 추적할 수 없다.<br>
다만 OffMeshLink를 사용해 **서로 분리된 메시를 연결하여 경로를 만들어줄 수 있다.** ⭐<br>
<br>

👉 **첫 번째 방법**<br>

![생성된오프메시](https://user-images.githubusercontent.com/43705434/127989409-18038b78-d8df-4a97-9bb1-c538e136ed7d.PNG)<br>
<br>

Bake 시점에 NavMesh의 **높낮이 차이, 떨어진 거리의 차이로 부터 연결을 생성**하는 방식이다.<br>
높낮이의 차이가 설정해준 값 이하인 경우에만 메시간의 연결이 생성된다.<br>

Navigation 탭에서 대상 오브젝트의 Generate OffMeshLink를 체크해주고 Drop Height, Jump Distance를 설정해준 후<br>
OffMeshLink를 생성하고자하는 오브젝트의 OffMeshLink Generation static을 체크해주어야 한다.<br>
<br>
<br>

👉 **두 번째 방법**<br>

![컴포넌트오프메시링크생성된거](https://user-images.githubusercontent.com/43705434/127989429-03513b7e-8dfa-4f7d-8264-3945972d66d0.PNG)<br>
<br>

연결되길 원하는 두 지점을 직접 지정하는 방식으로, 컴포넌트를 이용하기 때문에 Bake 할 필요가 없다.<br>
먼저 두 개의 게임 오브젝트를 만들고 연결되길 원하는 각 지점에 배치한다.<br>
이제 또 다른 게임 오브젝트에 Off Mesh Link 컴포넌트를 추가하고 Start, End 에 방금의 오브젝트들을 연결해주면 된다.<br>
<br>

👉 **Off Mesh Link 컴포넌트 설정**<br>

* Start : 시작 지점을 나타내는 오브젝트<br>

* End : 종료 지점을 나타내는 오브젝트<br>

* Cost Override : 경로 계산에 필요한 비용 (값이 양수 일 때만 적용)<br>

* Bi Directional : 할성화 되면 Start와 End 양방향 이동 가능<br>

* Activated	: 이 링크를 경로 계산에 사용할 지 여부<br>

* Auto Update Positions :  End 오브젝트의 위치가 바뀔 때 네비메시에 재 연결 될지 여부<br>

* Navigation Area : 네비메시 Area 레이어 설정<br>
<br>
<br>

👉 **Off Mesh Link 두가지 방식 정리**<br>
![오프메시링크2가지설정방법](https://user-images.githubusercontent.com/43705434/127989438-8dc1afcd-e56c-4100-addc-d186a3b7e06c.PNG)<br>
<br>
<br>

👉 **Off Mesh Link 두가지 방식 차이**<br>
![오프메시링크2가지방법차이](https://user-images.githubusercontent.com/43705434/127989401-dbbeebee-f61e-4475-bcab-e10c01467847.PNG)<br>
<br>
<br>

👉 **실사용 예시 2가지**<br>

**Off Mesh Link를 이용한 사다리 등반 코드 예시**<br>
![사다리등반](https://user-images.githubusercontent.com/43705434/127989419-2cfb0c03-2864-455b-91ef-233221f0564c.PNG)<br>
<br>
<br>

**Off Mesh Link를 이용한 점프 코드 예시**<br>
![오프메시링크점프](https://user-images.githubusercontent.com/43705434/127989454-32e1292f-d78f-49d1-bc84-6d9ea3c9260b.PNG)<br>
<br>
<br>

**위 두가지 예시를 사용하기 앞서 자동으로 OffMeshLink를 이동하지 않도록 NavMeshAgent 컴포넌트에서 Auto Traverse OffMeshLink를<br>
체크 해제해주어야 한다. 그 후 이동, 애니메이션 관련 코드를 직접 구현해야 한다.** ⭐⭐<br>
<br>
<br>
