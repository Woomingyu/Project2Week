using System.ComponentModel.Design;
using System.Drawing;

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


            //생성자 = 프로퍼티 => 생성자 => 클래스에서 해당 정보로 메서드 실행
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
        //4.인벤토리를 관리하는 클래스
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