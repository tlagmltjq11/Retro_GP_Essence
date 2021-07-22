## 🔔 UGUI
레거시 UI는 코드로 직접 생성해줘야 했어서, 눈으로 확인하며 작업하는 것이 불가능했다.<br>
하지만 ⭐ **UGUI는 UI요소를 게임 오브젝트 & 컴포넌트처럼 다루고 편집** ⭐ 할 수 있도록 하기 때문에<br>
비교적 간편하게 UI 작업이 가능하다.<br>
<br>
<br>

## 🔔 Canvas
모든 UI를 관리하는 Screen으로 ⭐ **UI 요소들이 배치되기 위한 프레임** ⭐ 역할을 한다.<br>
-> (배치를 위한 **스크린 좌표를 제공함.**)<br>
<br>

* 캔버스가 엄청 큰 것을 알 수 있다! -> 캔버스는 ⭐ 게임 화면(게임창)에 대응하기 때문.<br>
* 캔버스의 1px = 유니티 게임 월드에서 1m<br>
* 캔버스의 크기는 게임 화면 해상도와 같다.<br>
<br>

더 자세한 정보는 [다음 파일](https://github.com/tlagmltjq11/Retro_GP_Essence/blob/main/04.%20%EC%86%8C%EC%BD%94%EB%B0%98(%EC%B0%BD%EA%B3%A0%EC%A7%80%EA%B8%B0%20%EA%B2%8C%EC%9E%84)%20%EB%A7%8C%EB%93%A4%EA%B8%B0.md)을 참고.<br>
<br>
<br>

## 🔔 Rect Transform
모든 UI요소는 Rect Transform를 갖는데, 그 이유는 UI는 3차원 좌표 상이 아닌 ⭐ **사각형 판 위에서 배치되기 때문이다.**<br>
<br>
<br>

👉 **Anchor**<br>
⭐ **화면 프레임상**에서 특정 **UI 요소의 원점 위치**를 정하는 요소<br>

앵커를 프레임의 중앙으로 설정하고, 포지션을 (0, 0)으로 설정하면 정가운데 위치하게 된다.<br>

<img src="https://user-images.githubusercontent.com/43705434/126132952-23146339-262d-428a-b6b4-050c23506e3d.PNG" width="650" height="350"><br>
<br>

앵커를 프레임 상 (1,1)로 설정하면 오른쪽 위 꼭짓점이 된다. 그 상태에서 포지션을 (0, 0)으로 설정한 상태<br>

<img src="https://user-images.githubusercontent.com/43705434/126132956-c0d6c44a-92b3-4cb3-8d3f-681fe00d27a8.PNG" width="650" height="350"><br>
<br>
<br>

👉 **Pivot**<br>
⭐ **특정 UI 내부**에서 UI **자신을 배치하는 기준점**을 정하는 요소<br>

앵커는 프레임의 중앙 포지션은 (0, 0)이며, 피봇을 UI 내부의 중앙인 (0.5, 0.5)으로 설정한 상태<br>
<img src="https://user-images.githubusercontent.com/43705434/126132957-e107cf37-bfa8-40cb-b618-7941efd07344.PNG" width="650" height="350"><br>

앵커는 프레임의 중앙 포지션은 (0, 0)이며, 피봇을 UI 내부의 우측 상단인 (1, 1)으로 설정한 상태<br>
<img src="https://user-images.githubusercontent.com/43705434/126132959-683c9394-0ebf-464a-8f52-8e8955fd4173.PNG" width="650" height="350"><br>
<br>
<br>

👉 **Position**<br>
⭐ **앵커와 피봇을 기준으로 배치되는 최종 위치**<br>
<br>
<br>

👉 **앵커의 Min와 Max가 같지 않은 경우**<br>
-> 보통 같은 경우는 **절대값**으로 사용되지만 Min, Max가 다르다면 ⭐ **특정 영역을 나타내는 상대값**으로 사용된다.<br>

만약 Min = 0.2, Max = 0.8로 설정했다면 캔버스의 크기가 변할 때 min의 좌표라고 볼 수 있는 x 20%, y 20% 좌표와<br>
max의 좌표라고 볼 수 있는 x 80%, y 80% 좌표를 이은 직사각형 모양으로 캔버스 크기에 맞게 상대적으로 변한다.<br>
Min != Max 인 경우 PosX, PosY, Width, Height값은 의미가 없다. <br>
패딩에 따라, 캔버스 크기에 따라 크기와 위치가 상대적이기 때문이다.<br>
-> 따라서 Rect Transform 에서 없어짐.<br>
<br>
<br>

👉 **Anchor Presets**<br>
앵커 프리셋 : **유니티에서 제공하는 자주 사용되는 앵커-피봇 유형들이다.**<br>

<img src="https://user-images.githubusercontent.com/43705434/126133075-8235616f-42f9-43f4-8590-8abcbea2315f.PNG" width="400" height="450"><br>
<br>
<br>

👉 **마우스 좌클릭으로 선택**<br>
-> 앵커 위치만 바뀐다.<br>

👉 **Alt + 마우스 좌클릭으로 선택**<br>
-> 앵커 위치와 함께 실제 UI 요소의 위치도 그쪽으로 바뀐다.<br>

👉 **Shift +  마우스 좌클릭으로 선택**<br>
-> 앵커 위치와 함께 UI 요소의 피봇(기준점)도 그쪽으로 바뀐다.<br>

👉 **Alt + Shift + 마우스 좌클릭으로 선택**<br>
-> 앵커 + 피봇 + 실제 UI 요소의 위치 모두 다 그쪽으로 바뀐다.<br>

👉 **stretch**<br>
쫙 펴진 상태로 min != max 의 경우로 변한다.<br>
<br>
<br>

## 🔔 ETC

👉Visual Component<br>
**text, image, raw image ..** 등 그래픽을 그려내는 UI 요소들을 의미한다.<br>
<br>
<br>

👉 Interaction Component<br>
Visual Component들을 이용해서 구성한 **사용자 입력에 대응 가능한** UI 요소들을 의미한다.<br>
(**Button, Toggle, Slider, DropDown ...**)<br>
모든 Interaction Component는 **Selectable** 클래스를 상속받아 만들어졌다.<br>
-> Interactable, Transition, Navigation 등의 기능 사용가능.<br>
<br>
<br>

👉 Layout<br>
자식 오브젝트들의 정렬을 돕는 컴포넌트들..<br>
Vertical Layout Group - 수직 정렬<br>
Horizontal Layout Group - 수평 정렬<br>
Grid Layout Group - 격자 정렬<br>
Layout Element - 레이아웃의 구성요소로서 **특별히 필요한 것이 있다면 추가**하여 사용하는 컴포넌트<br>
<br>
<br>

👉 **On Value Changed와 같은 이벤트 메소드** 같은 경우 **Dynamic**과 **static**을 선택할 수 있는데 차이점은 다음과 같다.<br>
Dynamic<br>
유니티 이벤트가 입력 값(bool)을 전달할 수 있는 경우<br>
Play 중에 **자동으로 입력 값이 들어온다.**<br>
<br>

Static<br>
개발자가 직접 입력 값(bool)을 미리 조정 하는 경우<br>
<br>
<br>

👉 EventSystem<br>
UI Interaction의 과정은 다음과 같다.<br>
1. Canvas에 부착되어있는 Graphic Raycaster 컴포넌트를 통해 사용자의 마우스 클릭 위치에 광선을 쏜다.<br>
2. EventSystem이 광선과 충돌된 UI 오브젝트에게 **검출된 이벤트를 전송한다.**<br>
3. UI 오브젝트는 전송받은 이벤트를 실행한다.<br>

고로 EventSystem이 없거나 UI 오브젝트들이 Ray를 받아낼 수 없는 상황이라면 제대로된 UI 상호작용이 이루어지지 않을 것이다.<br>
<br>
<br>
