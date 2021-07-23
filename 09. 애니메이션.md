## 🔔 Mecanim
유니티에서 지원하는 ⭐ **애니메이션 시스템을 메카님**이라고 한다.<br>
이러한 메카님은 **인간형 캐릭터 애니메이션 제작에 주안점**을 두고 있으며, ⭐ **FSM 시스템을 이용하여<br>
애니메이션의 재생을 관리하고, 트랜지션 로직을 사용해 다른 상태로 넘어갈 수 있다.** ⭐<br>
이 메카님 시스템은 기본적인 애니메이션 기능은 물론 애니메이션 레이어, 애니메이션 블렌드,<br>
애니메이션 리타게팅 등의 다양한 기능을 제공한다.<br>
<br>
<br>

## 🔔 Animation Clip
**오브젝트가 어떻게 움직여야 하는지에 대한 정보**들이 포함된 것이 애니메이션 클립이다.<br>

이러한 애니메이션 클립을 만드는 방법은 크게 두 가지가 있다.<br>
👉 첫 번째는 3ds Max나 Maya 같은 **외부의 프로그램**으로 애니메이션을 만들어서 임포트 하는 것이고,<br>
👉 다른 하나는 **유니티에서 직접** 애니메이션 키를 잡아서 클립을 만드는 것이다.<br>
(Unity에서 직접 만드는것은 **간단한** 애니메이션이나 UI 애니메이션을 만들 때 사용되는 경우가 많다.)<br>
<br>
<br>

## 🔔 Animator Controller Asset
⭐⭐ **현재 어떤 애니메이션 클립을 실행하며, 특정 조건이 만족됐을 때 어떤 애니메이션 클립으로 전이할지 규칙을<br>
설계하는 state Machine 이다.** ⭐⭐<br>
(그 외에도 많은 기능들이 존재)<br>
<br>
<br>

## 🔔 Animator Component
Animation Clip과 Animator Controller를 이용해 오브젝트에 애니메이션 시스템을 할당하도록 돕는 컴포넌트.<br>
<br>
<br>


## 🔔 Rigging
리깅이란 모델링 캐릭터에 ⭐ **뼈대를 만들어 할당하여 캐릭터(Mesh)의 움직임을 제어하는 작업** ⭐을 말한다.<br>
이러한 리깅의 타입은 다음과 같이 2가지로 나뉜다.<br>

👉 **Humanoid**<br>
⭐ 인간형의 뼈대를 가진 대상.<br>

![humanoid](https://user-images.githubusercontent.com/43705434/126112936-2f6e3bf9-e8f3-4684-8427-80ea6c448af4.PNG)<br>
<br>
<br>

👉 **Generic**<br>
대표적으로 4족 보행 동물의 뼈대를 가진 대상.<br>

![generic_rig](https://user-images.githubusercontent.com/43705434/126112941-783dfe87-b67a-4eaf-9e5a-d5485969f3fe.PNG)<br>
<br>
<br>

👉 **Humanoid vs Generic 차이점**<br>
Humanoid형 애니메이션은 ReTargetting(리타겟팅) 시스템을 사용해서 공유 할 수 있다.<br>
(⭐ **ReTargetting : 뼈대 즉 Type이 같다면(인간형) 애니메이션을 공유해서 사용할 수 있는 것.** ⭐)<br>

Generic은 대체적으로 리타겟팅 기능이 동작하지 않지만, 완전히 동일한 rig 를 가지고 있다면 재사용 가능하다.<br>
<br>
<br>

## 🔔 Avartar
보편적인 3D 애니메이션을 3D 모델이 받아들일 수 있도록 도와주는 Born<br>
-> 타입만 같다면 아바타에 맞춰서 재생할 수 있도록 함.<br>

다른 본 구조 가진 캐릭터를 통합하는 기본 모델로 기본 모델 형태로 변경된 캐릭터를 아바타라 한다.<br>
(하나의 본 구조를 다른 본 구조로 리타겟팅 하기 위한 인터페이스)<br>

<br>
<br>

## 🔔 Layer

<img src="https://user-images.githubusercontent.com/43705434/126112929-2858c187-db4d-411e-9ea5-65d2dbc110ef.PNG" width="650" height="250"><br>
<br>

👉 Animation Layer를 사용하여 ⭐ **각 신체 부위에 대하여 복잡한 스테이트 머신들을 따로 관리할 수 있다.**<br>
예를 들어, 하체 레이어에서 걷거나 점프하고, 상체 레이어에서 오브젝트를 던지거나 쏘는 것과 같은 경우가 있다.<br>
즉, 신체 부위에 따라 각자 Animation FSM을 따로 적용하고자 한다면 Layer를 나누어서 사용하면 된다는 것이다.<br>
-> ⭐ 여러개의 상태를 동시에 동작시킬 수 있게되는 것.<br>

👉 경우에 따라서는 같은 스테이트 머신을 다른 레이어에서 재사용 할 수 있다.<br>
예를 들어 “부상당한 상태”의 거동을 재현하려는 데 “부상당한 상태”로 걷기/달리기/점프 애니메이션을<br>
“건강한 상태”의 각 애니메이션 대신 사용하고 싶은 경우 레이어들 중 하나의 Sync 체크박스를 클릭한 후,<br>
그 레이어와 동기화 할 레이어를 선택하면, 스테이트 머신의 구조는 동일해지지만<br>
각 스테이트에서 사용되는 애니메이션 클립은 교체된다.<br>
<br>

사용자는 각 레이어에 대해서 마스크(애니메이션이 적용될 부위)와 블렌딩 타입을 지정할 수 있다.<br>
<br>

👉 **Mask**<br>
해당 Layer의 ⭐ **애니메이션들이 적용될 부위**를 결정할 수 있다.<br>
![mask](https://user-images.githubusercontent.com/43705434/126112934-14894b8b-7647-4bff-93ef-6d6e35c22a88.PNG)<br>
<br>

👉 **Blending Type**<br>
**Additive** 는 이전 레이어어로부터 추가가 되는 반면에, **Override** 는 다른 레이어로부터의 정보가 전부 무시되고 덮어씌운다.<br>
⭐ 두 레이어에서 동작에 있어 겹치는 부분이 있다면 최종적으로 가장 아래에 있는 레이어가 덮어 씌운다.<br>
⭐ weight 값이 높을수록, 즉 가중치가 높을 수록 덮어 씌우는 정도가 높아진다.<br>
<br>
<br>

## 🔔 Blend Tree

<img src="https://user-images.githubusercontent.com/43705434/126112943-2dad911e-c9cf-486d-a3ad-0ff7baf1f5f6.PNG" width="450" height="300"><br>
<img src="https://user-images.githubusercontent.com/43705434/126112946-8de6c998-779f-4e92-b549-b184634de6ca.PNG" width="300" height="450"><br>
<br>

다양한 종류의 애니메이션을 제작하면 캐릭터의 움직임은 더욱 자연스러워 질 것이다.<br>
예를 들어 캐릭터가 움직이는 방향에 따라서 정면으로 움직일 때는 바르게 걷고, 정면을 본체로 왼쪽으로 움직이면<br>
왼쪽으로 옆걸음을 하는 방식으로 제작한다면 만약 왼쪽 정면 대각선 방향을 향해서 이동하면 어색한 움직임을 보일 것이다.<br>
그것을 자연스럽게 처리하려면 왼쪽 정면 대각선 방향으로 움직이는 애니메이션을 만들어주어야 하고, 또 거기서 더 자연스러운<br>
애니메이션을 만들고자 한다면 더 세분화하여 여러 애니메이션을 만들어서 추가해주어야 하는 것이다.<br>
하지만 그렇다고 해서 게임 제작자가 **모든 경우의 애니메이션을 일일이 다 만들 수는 없다.**<br>
그런 작업은 쉽지 않을 뿐만 아니라 게임제작에 들어가는 자원과 시간이 부족하다.<br>

이러한 문제를 해결하기 위해서 사용되는 기능이 바로 애니메이션 블랜드이다.<br>
⭐⭐ **블렌드 트리는 여러 애니메이션을 적절히 섞어 자연스럽게 동시에 재생될 수 있도록 해주는 기능이다.<br>
(파라미터 값에 따라서 어떠한 애니메이션이 더 반영되어야 하는지 결정되어 자연스럽게 블렌딩된다.)** ⭐⭐<br>
<br>
<br>

출처: https://wergia.tistory.com/54 [베르의 프로그래밍 노트]<br>
<br>
<br>

## 🔔 IK
어떤 위치를 중심으로 관절의 위치를 **역계산** 하는 것.<br>
물체의 위치가 어디든 상관없이 거기에 맞춰서 시선이나 팔의 위치가 결정될 때 많이 쓴다.<br>
(아이템을 획들할 때, 아이템의 위치에 따라 팔을 뻗는 위치가 달라지게 한다는 둥..)<br>
<br>

📜<br>
```c#
    void OnAnimatorIK()  
    {
        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0.5f);  
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0.5f); 

        anim.SetIKPosition(AvatarIKGoal.LeftHand, target.position);  
        anim.SetIKRotation(AvatarIKGoal.LeftHand, target.rotation); 

        anim.SetLookAtWeight(1.0f);
        anim.SetLookAtPosition(target.position);
    }
```
<br>
<br>
