namespace _2주차_개인과제
{
    public class Position
    {
        //모든 객체의 위치정보 변수
        public int x { get; set; }
        public int y { get; set; }
        public char sym { get; set; }



        //위치 정보&기호 저장
        public Position(int _x, int _y, char _sym)
        {
            x = _x;
            y = _y;
            sym = _sym;
        }


        //대상 위치의 객체 출력, 제거
        public void Draw()
        {
            Console.SetCursorPosition(x, y);
            Console.Write(sym);
        }
        public void Clear()
        {
            sym = ' ';
            Draw();
        }


        //닿으면 상호작용(전투/상점/던전입장/던전퇴장...)
        public bool IsHit(Position position)
        {
            return position.x == x && position.y == y;
        }
    }



    public class Player
    {
        public Player(Position position)
        {
            Position playerPosition = new Position(position.x, position.y, '㉾');
        }


        //플레이어 상호작용
        public bool MonsterHit(Position Monster)
        {
            //몬스터의 포지션 값과 내 포지션 값이 닿았는지 확인
            Position PlayerMove = GetNextPoint();
            if (PlayerMove.IsHit(Monster))
            {
                Monster.sym = PlayerMove.sym; //아이콘 변경
                //인벤토리의 장비 정보와 && 체력/방어력/공격력을 계산해서 스테이터스에 반환
                return true;
            }
            else
            {
                return false;
            }
        }

        //움직이는 키 누르면 바로 이전 위치를 지우고 넘어가면 이동 구현이 될 것 같은데??
        public void Move()
        {
            Point tail = body.First();
            body.Remove(tail);
            Point head = GetNextPoint();
            body.Add(head);
            //
            tail.Clear();
            head.Draw();
        }

        //##실제로 뱀이 다음에 이동할 위치를 반환하는 메서드.##

        //Point클래스를 head로 정의하고 거기 body 리스트의 마지막 항목을 넣어줌
        //Point nextPoint(Point 생성자)에게 head의 x y sym 값을 넣어줌
        //스위치문에 class Snake가 받아온 enum값에 따라 실제 방향값을 바꿔줌
        //좌 우 하 상
        //direction 입력이 없는경우 현재의 이동값을 유지함
        //이전에 바꾼 newxtPoint.x,y의 값은 변하지 않으므로 한 방향으로 이동하게 됨
        public Point GetNextPoint()
        {
            Point head = body.Last();
            Point nextPoint = new Point(head.x, head.y, head.sym);
            switch (direction)
            {
                case Direction.LEFT:
                    nextPoint.x -= 2;
                    break;
                case Direction.RIGHT:
                    nextPoint.x += 2;
                    break;
                case Direction.UP:
                    nextPoint.y -= 1;
                    break;
                case Direction.DOWN:
                    nextPoint.y += 1;
                    break;
            }
            return nextPoint;
        }

        // ##뱀이 자신의 몸에 부딪혔는지 확인하는 메서드##

        // 임시 자료형 head에 body리스트의 마지막 항목을 넣어줌
        // 몸 개수 3개를 기준으로 1회, 늘어는 만큼 반복한다.
        // IsHit에 i번째 body의 위치값을 비교시켜 동일한 값(true)가 반환되면
        // IsHitTall의 호출자에게 true, 반대의 경우 false를 반환
        public bool IsHitTail()
        {
            var head = body.Last();
            for (int i = 0; i < body.Count - 2; i++)
            {
                if (head.IsHit(body[i]))
                    return true;
            }
            return false;
        }

        //##뱀이 벽에 부딪혔는지 확인하는 메서드##

        //IsHitTail과 비슷한 메서드이지만, 벽의 위치는 고정되어 있으므로
        //벽의 값만 조건문에 넣어 참거짓 return을 준다.
        public bool IsHitWall()
        {
            var head = body.Last();
            if (head.x <= 0 || head.x >= 80 || head.y <= 0 || head.y >= 20)
                return true;
            return false;
        }
    }

    //##FoodCreator 클래스에 포함된 정보 목록## 적어서 생략
    public class FoodCreator
    {

        //맵의 크기와, 음식의 형태가 될 문자 변수 선언
        int mapWidth;
        int mapHeight;
        char sym;

        //랜덤 변수 선언
        Random random = new Random();

        //##FoodCreator 생성자##
        //맵의 크기와 형태가 될 문자를 받아오게 함
        public FoodCreator(int mapWidth, int mapHeight, char sym)
        {
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
            this.sym = sym;
        }

        // #무작위 위치에 음식을 생성하는 메서드#
        //x, y값을 맵 안쪽에 들어오도록 조정함
        //x 좌표를 2단위로 맞추기 위해 짝수로 만듬? 왜????
        //Point 클래스에 결과로 나온 x, y 좌표와 FoodCreator의 sym을 반환함
        public Point CreateFood()
        {
            int x = random.Next(2, mapWidth - 2);
            // x 좌표를 2단위로 맞추기 위해 짝수로 만듭니다.(가로 이동 두칸씩 해야 세로랑 비율이 맞음)
            x = x % 2 == 1 ? x : x + 1; //? 앞에까지가 if문의 조건문

            int y = random.Next(2, mapHeight - 2);
            return new Point(x, y, sym);
        }
    }







    //실제로 게임을 실행하는 클래스
    internal class Program
    {
        static void Main(string[] args) //이게 스타트이자 업데이트 메서드라고 생각하자
        {

            //!!게임 시작 기본 배치!!


            // 게임 속도(값이 클수록 게임이 느려짐=>점점 줄어들어야 함)
            int gameSpeed = 100;

            // 먹은 음식 수(이 값에 따라 뱀의 길이가 길어져야 함)
            int foodCount = 0;

            // 게임 시작 시 벽을 그려주는 메서드
            DrawWalls();

            // 뱀의 초기 위치를 설정, 그린다(생성자Point 호출/원하는 좌표&해당 좌표에 들어갈 문자)
            Point p = new Point(4, 5, '*');

            //초기 뱀의 꼬리 Point, 뱀의 길이, 방향을 입력
            Snake snake = new Snake(p, 4, Direction.RIGHT);

            //각 몸통(머리포함)의 위치에 맞기 그려준다.(Snake -> Point)
            snake.Draw();

            // 음식생성 클래스인FoodCreator의 생성자에게 맵의 크기와 음식의 문자값을 알려줌
            FoodCreator foodCreator = new FoodCreator(80, 20, '$');

            //food라는 포인트 클래스 변수를 생성 => 랜덤 위치값&음식 기호를 지정해주는 함수를 넣음
            Point food = foodCreator.CreateFood();

            //CreateFood()에서 받아온 값을 통해 랜덤 위치에 음식을Draw 한다
            food.Draw();


            // 게임 루프: 이 루프는 게임이 끝날 때까지 계속 실행됩니다.(이 안은 update 함수와 동일 기능)
            while (true)
            {
                // 키 입력이 있는 경우에만 방향을 변경합니다.
                if (Console.KeyAvailable)
                {
                    //키 입력을 받아 해당 키를 true로 만든다
                    var key = Console.ReadKey(true).Key;

                    //스위치 문을 통해 각 키에 enum Direction의 값을 넣어준다.
                    //입력상태 확인 -> 입력된 키값 인식 -> 입력된 키에 Direction 배정 
                    switch (key)
                    {
                        case ConsoleKey.UpArrow:
                            snake.direction = Direction.UP;
                            break;
                        case ConsoleKey.DownArrow:
                            snake.direction = Direction.DOWN;
                            break;
                        case ConsoleKey.LeftArrow:
                            snake.direction = Direction.LEFT;
                            break;
                        case ConsoleKey.RightArrow:
                            snake.direction = Direction.RIGHT;
                            break;
                    }
                }

                // 뱀이 음식을 먹었는지 확인합니다.(Snake.Eat 에 food의 좌표 보내줘서 판별)
                if (snake.Eat(food))
                {
                    // 먹은 음식 수를 증가
                    foodCount++;

                    //CreateFood()에서 받아온 값을 통해 랜덤 위치에 음식을Draw 한다?
                    //food의 위치에 새로운 head를 그린다.
                    food.Draw();

                    // 뱀이 음식을 먹었다면, 새로운 음식을 만들고 그립니다.(시작 설정과 동일한 food.Draw)
                    food = foodCreator.CreateFood();
                    food.Draw();

                    //음식을 먹은 경우 게임 속도를 점점 올립니다.
                    if (gameSpeed > 10) // 게임이 점점 빠르게
                    {
                        gameSpeed -= 10;
                    }


                }
                else
                {
                    // 뱀이 음식을 먹지 않았다면, 그냥 이동.
                    snake.Move();
                }

                //일시 정지 메서드지만 밀리초 단위 정지이므로 여기서는 게임 속도처럼 사용했다.
                Thread.Sleep(gameSpeed);

                // 벽이나 자신의 몸에 부딪히면 게임을 끝냄
                if (snake.IsHitTail() || snake.IsHitWall())
                {
                    break; //update를 대체하는 while문에서 탈출
                }

                //먹은 음식 수 출력 위치&출력
                Console.SetCursorPosition(0, 21);
                Console.WriteLine($"먹은 음식 수: {foodCount}");
            }

            WriteGameOver();  // 게임 오버 메시지를 출력합니다.
            Console.ReadLine(); //즉시 종료되는걸 방지하기 위해서
        }


        //##메인 클래스의 함수들##


        //메인 호출순서 1.벽 그리는 메서드
        static void DrawWalls()
        {
            // 상하 벽 그리기
            //가로로 80개의 #을 상(20),하(0) 기준으로 그린다. 
            for (int i = 0; i < 80; i++)
            {
                Console.SetCursorPosition(i, 0);
                Console.Write("#");
                Console.SetCursorPosition(i, 20);
                Console.Write("#");
            }

            // 좌우 벽 그리기
            //세로로 80개의 #을 좌(0),우(80) 기준으로 그린다. 
            for (int i = 0; i < 20; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write("#");
                Console.SetCursorPosition(80, i);
                Console.Write("#");
            }
        }

        //게임오버 텍스트
        static void WriteGameOver()
        {
            int xOffset = 25;
            int yOffset = 22;
            Console.SetCursorPosition(xOffset, yOffset++);
            WriteText("============================", xOffset, yOffset++);
            WriteText("         GAME OVER", xOffset, yOffset++);
            WriteText("============================", xOffset, yOffset++);
        }

        //텍스트의 위치를 잡아주고, 실제 출력하는 함수
        static void WriteText(string text, int xOffset, int yOffset)
        {
            Console.SetCursorPosition(xOffset, yOffset);
            Console.WriteLine(text);
        }

    }

    //방향을 표기하는 열거형
    //Enum상수들의 집함을 의미하는 class임
    //이게 뭔소리냐?
    //== 아래 메서드는 사실 class Direction과 같으며,
    //내용물은 public static final Direction LEFT = new Direction(); 이 생략되어 아래와 같이 요약된거임
    public enum Direction
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }
}