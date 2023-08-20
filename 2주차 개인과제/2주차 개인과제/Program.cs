using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using static _2주차_개인과제.Program.Item;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace _2주차_개인과제
{

    //실제로 게임을 실행하는 클래스
    internal class Program
    {
        //필요한 것
        //1.커서의 위치를 바꿔주고, 그 자리에 출력해주는 클래스
        public class CursorPosition
        {
            //커서 위치 자동 프로퍼티(좌표 x,좌표 y,문자 형태)
            public int x { get; set; }
            public int y { get; set; }
            public char sym { get; set; }


            //프로퍼티 => 생성자 => 클래스에서 해당 정보로 메서드 실행
            public CursorPosition(int _x, int _y, char _sym)
            {
                x = _x;
                y = _y;
                sym = _sym;
            }

            // 생성 메서드
            public void Draw()
            {
                Console.SetCursorPosition(x, y); //커서 위치를 직업 옮겨주는 메서드
                Console.Write(sym);
            }

            // 제거 메서드
            public void Clear()
            {
                sym = ' ';
                Draw();
            }

            // 두 점이 같은지 비교하는 메서드
            public bool Collision(CursorPosition p)
            {
                return p.x == x && p.y == y;
            }
        }




        //2.플레이어의 정보를 담는 클래스(위치, 이동, 충돌, 스테이터스 까지?)

        //##Snake 클래스에 포함된 정보 목록##

        //1.Snake의 생성자(값 초기화, body 리스트 생성, ?? +=1)
        //2.body의 리스트 정보를 Point.Draw에 전달하는 Draw
        //3.뱀이 음식에 닿았는지 판별하고, 먹은 음식을 지우는 Eat
        //4.뱀의 이동을 구현하는 Move
        //5.뱀의 이동방향을 결정하는 GetNextPoint
        //6.뱀의 피격(게임 종료)를 불러올 IsHitTail && IsHitWall
        //플레이어 캐릭터(뱀) 자체를 관리하는 클래스
        public class Player
        {
            //변수
            public CursorPosition playerCurrentPosition;
            public Direction direction;


            public Player(CursorPosition playerPos)
            {
                playerCurrentPosition = new CursorPosition(playerPos.x, playerPos.y, '＠');

            }

            //플레이어 위치설정 : 생성자 메서드 // Main호출 -> playerDraw() -> cursor Class -> 실제 출력 Draw()
            public void PlayerDraw(CursorPosition playerDrawPos)
            {
                playerDrawPos.Draw();
            }


            // ##뱀이 이동하는 메서드##

            //tail에 body리스트의 첫번째 항목을 꺼내 넣습니다.
            //body 리스트에서 tail을 제거합니다.
            //head 에 다음에 이동할 위치를 반환시켜 넣습니다.
            //body 리스트에 head를 추가합니다.
            //tail의 출력 값을 지우고(공란으로 덮어씌우고)
            //head를 그려줍니다???(class Snake)
            public void Move()
            {
                CursorPosition playerPos = playerCurrentPosition;
                CursorPosition playerNextPos = GetNextPoint();

                playerPos.Clear();
                playerNextPos.Draw();
            }

            //##실제로 뱀이 다음에 이동할 위치를 반환하는 메서드.##

            //Point클래스를 head로 정의하고 거기 body 리스트의 마지막 항목을 넣어줌
            //Point nextPoint(Point 생성자)에게 head의 x y sym 값을 넣어줌
            //스위치문에 class Snake가 받아온 enum값에 따라 실제 방향값을 바꿔줌
            //좌 우 하 상
            //direction 입력이 없는경우 현재의 이동값을 유지함
            //이전에 바꾼 newxtPoint.x,y의 값은 변하지 않으므로 한 방향으로 이동하게 됨
            public CursorPosition GetNextPoint()
            {              
                CursorPosition idle = playerCurrentPosition;
                CursorPosition nextPos = new CursorPosition(playerCurrentPosition.x, playerCurrentPosition.y, playerCurrentPosition.sym);
                switch (direction)
                {
                    case Direction.LEFT:
                        nextPos.x -= 2;
                        break;
                    case Direction.RIGHT:
                        nextPos.x += 2;
                        break;
                    case Direction.UP:
                        nextPos.y -= 1;
                        break;
                    case Direction.DOWN:
                        nextPos.y += 1;
                        break;
                }
                return idle; //이거 버그날 수 있음 ★★★
            }



            //플레이어가 몬스터에 닿았는지 확인하는 메서드
            public bool CollisionMonster(CursorPosition monster)
            {
                CursorPosition playerNextPos = GetNextPoint();
                if (playerNextPos.Collision(monster))
                {
                    monster.sym = playerNextPos.sym;
                    //몬스터와 닿은 경우의 여러 수를 생각하면 될듯
                    //if(몬스터가 더 약한경우) = 내 체력 감소(방어력 계산), 아이템 획득, 골드 획득
                    //else 몬스터가 더 강한경우 (바로 게임오버 실행) = 게임오버가 될 경우가 이거 뿐이므로 player에 넣자
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // ##뱀이 자신의 몸에 부딪혔는지 확인하는 메서드##

            // 임시 자료형 head에 body리스트의 마지막 항목을 넣어줌
            // 몸 개수 3개를 기준으로 1회, 늘어는 만큼 반복한다.
            // IsHit에 i번째 body의 위치값을 비교시켜 동일한 값(true)가 반환되면
            // IsHitTall의 호출자에게 true, 반대의 경우 false를 반환



            //벽(□(벽)에 닿을경우 이전 위치로 돌리는것도 괜찮을듯?)
            public bool IsHitWall() //호출자에게 벽을 넘을 수 없습니다
            {
                CursorPosition playerPos = playerCurrentPosition;


                if (playerPos.x <= 0)
                {
                    playerPos.x = 1;
                    return true;
                }
                else if (playerPos.x >= 80)
                {
                    playerPos.x = 79;
                    return true;
                }
                else if (playerPos.y <= 0)
                {
                    playerPos.y = 1;
                    return true;
                }
                else if (playerPos.y >= 20)
                {
                    playerPos.y = 79;
                    return true;
                }
                else
                return false;
            }

            /*
            public bool IsHitWall2() //호출자에게 벽을 넘을 수 없습니다. 메시지를 출력하게 함?
            {
                CursorPosition playerPos = GetNextPoint();
                CursorPosition playerBeforePos = playerCurrentPosition;

                if (playerPos.sym == '□')
                {
                    playerPos = playerBeforePos;
                    return true;
                }
                else
                    return false;
            }
            */
        }

        //방향
        public enum Direction
        {
            LEFT,
            RIGHT,
            UP,
            DOWN
        }
        //3.아이템의 정보를 담는 클래스

        public class Item
        {
            public string[] itemName = { "낡은 칼", "평범한 칼", "좋은 칼", "낡은 갑옷", "평범한 갑옷", "좋은 갑옷" }; // 아이템 이름
            public string[] itemDesc = { "낡은 칼이다.", "평범한 칼이다.", "좋은 칼이다", "낡은 갑옷이다.", "평범한 갑옷이다.", "좋은 갑옷이다" }; // 아이템의 설명
            public ItemType itemType; // 아이템 유형

            public string[] itemImage = { "ㅋ", "카", "칼", "ㄱ", "가", "갑" }; // 아이템 이미지 = 인벤토리 이미지용 
            
            public Item(Slot slot, int itemnum)
            {
                slot = itemName[itemnum];

            }
            public enum ItemType //열거형 아이템 타입
            { 
                Weapon, // 무기
                Armor, // 방어구
                //Used, // 소모품
            }

        }

        //4.인벤토리를 관리하는 클래스
        public class Inventory
        {
            public static bool inventoryActivated = false; //인벤토리 활성화 상태(i 입력/콘솔입력창 생성/활성화)

            public CursorPosition inventoryPos;
            Slot[] slots;

            //아이템 이름, 설명, 타입을 시작할 행 지점 x
            public int inventoryNamePos; 
            public int invertoryDescPos; 
            public int inventoryTypePos;

            //이따 UI위치에 맞게 수정 ★★★
            //아이템이 위치할 열 지점 (아이템 칸) y
            public int[] slotWeapon = {1, 2, 3, 4, 5};
            public int line = 0;
            public int[] slotArmor = {7, 8, 9, 10, 11};


            private bool isNotPut; //인벤토리 혹은 퀵슬롯이 가득찬 경우
            private int SlotNumber;
            public Inventory(Item.ItemType itemType)
            {
                if(itemType == ItemType.Weapon)
                {
                    inventoryPos.y = slotWeapon;
                }
            }

            /*
            //인벤토리 오픈 시도 (이거 메인으로 적절하게 수정해서 옮겨야함)★
            private void TryOpenInventory()
            {
                if (Input.GetKeyDown(KeyCode.I)) // i버튼 입력 시
                {
                    inventoryActivated = !inventoryActivated; //인벤토리 활성화 bool값 스위치

                    if (inventoryActivated) //활성화 상태로 해라
                        OpenInventory(); // 인벤토리 실제 오픈
                    else // 비활성화 상태로 해라
                        CloseInventory(); // 인벤토리 실제 닫기
                }
            }
            */

            //###아이템 채워넣기### ActionController에서 호출함!
            public void AcquireItem(Item _item) // 아이템과 개수 받아오기 개수 기본값 1
            {
                PutSlot(quickslots, _item, _count); //퀵슬롯부터 채우기/ 인벤토리부터 채우고 싶을경우 파라미터 값 slot으로 수정
                if (!isNotPut)
                    theQuickSlot.IsActivatedQuickSlot(SlotNumber);


                if (isNotPut)
                    PutSlot(slots, _item, _count); // 퀵슬롯이 가득 찬 경우 다음으로 인벤토리를 채우도록

                if (isNotPut)
                    Debug.Log("퀵슬롯과 인벤토리가 꽉찼습니다"); // 인벤토리 / 퀵슬롯이 가득 찬 경우 
            }


            private void PutSlot(Slot[] slots, Item item, int count)
            {
                if (item.itemType == Item.ItemType.Weapon) // 아이템 타입이 무기라면
                {
                    //아이템이 있는경우 반복
                    for (int i = 0; i < slotWeapon.Length; i++) // 슬롯 개수만큼 반복
                    {
                        if (slots[i] != null) //null이 아닐 때만 비교 ((안하면 아래에서 null을 참조하게 되므로))
                        {
                            if (_slots[i].item.itemName == _item.itemName) //i번째 슬롯이 가진 아이템 이름이 받아온 아이템의 이름과 같다면
                            {
                                SlotNumber = i;
                                _slots[i].SetSlotCount(_count); //받아온 개수만큼 SetSlotCount함수로 보내줌
                                isNotPut = false;
                                return;//끝
                            }
                        }

                    }
                }



                //아이템이 없는경우 반복
                for (int i = 0; i < _slots.Length; i++) // 슬롯 개수만큼 반복
                {
                    if (_slots[i].item == null) // 슬롯에 빈자리가 있다면 (null인경우 참조 적으면 안됨)
                    {
                        _slots[i].AddItem(_item, _count); // 해당 슬롯 AddItem에 아이템과 개수를 넘겨줌
                        isNotPut = false;
                        return;//끝
                    }
                }

                isNotPut = true;
            }
        }

        //4-2 인벤토리의 슬롯을 담당하는 클래스

        public class Slot
        { 
            public Item item; // 획득한 아이템;




            //##슬롯 영역값##
            private int slotNumber;
            public int num;
            

            //##모든 아이템 획득 함수##
            public void AddItem(Item _item, int itemNum) //아이템, 아이템 번호
            {
                item = _item; // 획득한 아이템 = 파라미터 _item 에 넣어줌
                itemDesc = itemDesc[itemNum];
                itemImage = itemImage[itemNum];

            }

            // 슬롯 초기화
            private void ClearSlot()
            {
                itemName = " ";
                itemDesc = " ";
                itemImage = " ";
            }

            //장비교체 // 아이템 클릭 시(장비교체, 소모품 사용)
            public void OnPointerClick(PointerEventData eventData)
            {
                //해당 스크립트가 적용된 객체에 우클릭을 하면 함수 실행
                //유니티에서 기본으로 불러왔을 뿐 printerEventData 부터  익숙하게 쓰던것과 동일
                // 클래스 안에있는.enum(열거형) 안에 있는. 변수자료 형식임
                if (eventData.button == PointerEventData.InputButton.Right) // 이벤트 버튼= 이벤트 버튼 우클릭
                {
                    if (item != null) // 아이템이 있는경우만 OnPointerClick
                    {
                        {
                            theItemEffectDatabase.UseItem(item);

                            if (item.itemType == Item.ItemType.Used) // 아이템타입이 소모품일 경우에만
                                SetSlotCount(-1); //개수 깎기

                        }
                    }
                }
            }

        }


        //5.몬스터를 담당하는 클래스
        //6.상점의 정보를 담당하는 클래스
        //나는 클래스 == 유니티의 cs 생성, Program 클래스 == Start() & Update() & 두 메서드에서 담당할 함수 포함 / 으로 생각하고 코드를 짜기로 함.
        //모든 상호작용 Collision으로 판별하기로 함    


        //실행 순서 => UI(틀) 과 맵 생성 => 각 클래스에서 정보를 받아 플레이어(고정)와 몬스터(랜덤) 그리고 출구를 생성 => 플레이어의 스테이터스와 인벤토리의 정보를 출력
        //콘솔창은 기본적으로 방향키 입력만을 받으며, 몬스터와 닿을 시 현재 체력과 방어력을 계산하여 승패를 판단
        //승리 시 아이템과 골드 획득 패배 시 게임 오버 출력 (공통) 몬스터의 공격력에 따라 체력을 감소 시킴
        //특정 키(i)를 누르면 인벤토리 제어 모드로 들어감 해당 모드에선 아이템의 장착/탈착이 가능함 (숫자 입력시 현재 착용 장비와 자동 스왑)


        //출구로 나가면 맵안의 내용물을 지우고 다시 배치(미정)
        static void Main(string[] args) //이게 스타트이자 업데이트 메서드라고 생각하자
        {

        }
    }


}