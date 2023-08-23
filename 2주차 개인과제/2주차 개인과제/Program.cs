using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace _2주차_개인과제
{
    //인벤토리의 배치 순서에 따라 해당 슬롯의 아이템 정보를 저장
    public class Slot
    {
        public Item SlotItem { get; set; }
        public int SlotNum { get; set; }
        public bool IsUsed { get; set; } // 아이템 사용 상태
        public bool IteminSlot { get; set; } // 슬롯 사용 상태

        public Slot(Item slotitem, int slotNum)
        {
            SlotItem = slotitem;
            SlotNum = slotNum;
            IsUsed = false;
            IteminSlot = false;
        }
    }

    internal class Program
    {
        static Character player; //플레이어 데이터 베이스// 전역에서 사용할 수 있게?
        static Item[] items; //아이템 데이터 베이스 배열
        static Slot[] inventorySlots;
        static List<string> purchasedItems = new List<string>(); // 이미 구매한 아이템 데이터

        static void Main(string[] args)
        {
            //Start()
            GameDataSetting(); //게임 데이터 세팅
            DisplayGameIntro(); //게임 인트로 세팅
        }

        //데이터 저장소(DataBase)
        static void GameDataSetting()
        {
            // 캐릭터 정보 세팅
            player = new Character("Chad", "전사", 1, 10, 5, 100, 1500);


            items = new Item[]
            {
            new Item("낡은 검", 2, "쉽게 볼 수 있는 낡은 검 입니다.", "무기", 500),
            new Item("평범한 검", 5, "평범한 검 입니다.", "무기", 1000),
            new Item("좋은 검", 20, "쉽게 볼 수 없는 좋은 검 입니다.", "무기", 3000),
            new Item("무쇠 갑옷", 5, "무쇠로 만들어져 튼튼한 갑옷입니다.", "방어구", 1500),
            new Item("사슬 갑옷", 8, "사슬로 만들어져 튼튼한 갑옷입니다.", "방어구", 2000),
            new Item("강철 갑옷", 12, "강철로 만들어져 튼튼한 갑옷입니다.", "방어구", 5000)
            };

        }

        static void DisplayGameIntro()
        {
            //인벤토리 배치(첫 실행 시 배치 / 아니면 기존 값 유지)
            if (inventorySlots == null)
            {
                // 초기화되지 않았다면 인벤토리 초기화 (람다식 사용 입력할 파라미터(item) => 조건에 따라 가져옴;) // 아이템에서 이름을 통해 아이템 가져오기
                inventorySlots = new Slot[]
                {
                    new Slot(items.FirstOrDefault(item => item.PickItem() == "낡은 검"), 1),
                    new Slot(items.FirstOrDefault(item => item.PickItem() == "무쇠 갑옷"), 2)
                };
            }


            Console.Clear();

            Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
            Console.WriteLine("이곳에서 전전으로 들어가기 전 활동을 할 수 있습니다.");
            Console.WriteLine();
            Console.WriteLine("1. 상태보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 상점");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");

            int input = CheckValidInput(1, 3);
            switch (input)
            {
                case 1:
                    DisplayMyInfo();
                    break;

                case 2:
                    DisplayInventory(inventorySlots); //inventorySlots : 인벤토리 슬롯의 상태를 매번 바꾸면서 저장까지 해야하는데 어떻게? => inventorySlots를 클래스 레벨에서 선언해서 초기화되지 않게 변경
                    break;
                case 3:
                    DisplayShop(); // 상점 화면 표시
                    break;
            }
        }


        //캐릭터 정보창(기본 능력치 + 장비 추가 능력치)
        static void DisplayMyInfo()
        {
            Console.Clear();

            //장비 능력치 초기화
            int WeaponAtk = 0;
            int ArmorDef = 0;


            // 장착된 아이템들을 확인하고 능력치를 표기(배열만큼 반복) / 이러면 착용 무기가 둘이건, 착용 방어구가 세개건 잘 작동할것
            //장비 능력치 계산
            foreach (var slot in inventorySlots)
            {
                if (slot.IsUsed)
                {
                    Item usedItem = slot.SlotItem;
                    if (usedItem.Type == "무기")
                    {
                        WeaponAtk += usedItem.Effect;
                    }
                    else if (usedItem.Type == "방어구")
                    {
                        ArmorDef += usedItem.Effect;
                    }
                }
            }

            Console.WriteLine("상태 보기");
            Console.WriteLine("캐릭터의 정보가 표시됩니다.");
            Console.WriteLine();
            Console.WriteLine($"Lv.{player.Level}");
            Console.WriteLine($"{player.Name}({player.Job})");
            Console.WriteLine($"공격력 :{player.Atk} (+{WeaponAtk})");
            Console.WriteLine($"방어력 : {player.Def}(+{ArmorDef})");
            Console.WriteLine($"체력 : {player.Hp}");
            Console.WriteLine($"Gold : {player.Gold} G");
            Console.WriteLine();
            Console.WriteLine("0. 나가기");

            int input = CheckValidInput(0, 0);
            switch (input)
            {
                case 0:
                    DisplayGameIntro();
                    break;
            }
        }

        //인벤토리
        static void DisplayInventory(Slot[] inventorySlots)// 인벤토리에 들어갈 아이템, 순서를 포함한 배열 Slot)
        {
            Console.Clear();

            Console.WriteLine("인벤토리");
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            Console.WriteLine();
            Console.WriteLine("[아이템 목록]");
            Console.WriteLine($"{"   아이템 이름",-15}{"효과",-10}\t\t{"설명",-30}\t{"  분류",-5}\t{" 가격",-10}");


            for (int i = 0; i < inventorySlots.Length; i++)
            {
                Slot slot = inventorySlots[i];
                Item inventoryItem = slot.SlotItem;

                string part = "";
                string useTxt = slot.IsUsed ? "[E]" : ""; //if, else

                if(!slot.IteminSlot)
                {
                    if (inventoryItem.Type == "무기")
                    {
                        part = "공격력+";
                    }
                    else if (inventoryItem.Type == "방어구")
                    {
                        part = "방어력+";
                    }

                    //Console.WriteLine($"{i + 1}.{useTxt} {inventoryItem.Name,-15}|{part}{inventoryItem.Effect,-10}|{inventoryItem.Desc,-30}|{inventoryItem.Type}|{inventoryItem.Price}G");



                    string itemInfo = $"{i + 1}.{useTxt} {inventoryItem.Name}\t|{part}{inventoryItem.Effect,-5}|{inventoryItem.Desc,-30}\t\t|{inventoryItem.Type}\t|{inventoryItem.Price}G";
                    Console.WriteLine(itemInfo);
                    slot.IteminSlot = true;
                }

            }

            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine("1. 아이템 장착/해제");
            Console.WriteLine("2. 아이템 정렬");

            int input = CheckValidInput(0, 2);
            switch (input)
            {
                case 0:
                    DisplayGameIntro();
                    break;
                case 1:
                    UsedorunUsed(inventorySlots);//장착 관리 메서드
                    break;
                case 2:
                    SortDisplay(inventorySlots);//정렬 관리 메서드
                    break;
            }
        }

        //아이템 장착
        static void UsedorunUsed(Slot[] inventorySlots)
        {
            Console.Clear();

            Console.WriteLine("인벤토리 - 장착/해제");
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            Console.WriteLine();
            Console.WriteLine("[아이템 목록]");
            Console.WriteLine($"{"   아이템 이름",-15}{"효과",-10}\t\t{"설명",-30}\t{"  분류",-5}\t{" 가격",-10}");


            for (int i = 0; i < inventorySlots.Length; i++)
            {
                Slot slot = inventorySlots[i];
                Item inventoryItem = slot.SlotItem;

                string part = "";
                string useTxt = slot.IsUsed ? "[E]" : "";

                if (inventoryItem.Type == "무기")
                {
                    part = "공격력+";
                }
                else if (inventoryItem.Type == "방어구")
                {
                    part = "방어력+";
                }

                //Console.WriteLine($"{i + 1}.{useTxt} {inventoryItem.Name,-15}|{part}{inventoryItem.Effect,-10}|{inventoryItem.Desc,-30}|{inventoryItem.Type}|{inventoryItem.Price}G");



                string itemInfo = $"{i + 1}.{useTxt} {inventoryItem.Name}\t|{part}{inventoryItem.Effect,-5}|{inventoryItem.Desc,-30}\t\t|{inventoryItem.Type}\t|{inventoryItem.Price}G";
                Console.WriteLine(itemInfo);
            }

            Console.WriteLine();
            Console.WriteLine("0. 인벤토리로 돌아가기");

            int input = CheckValidInput(0, inventorySlots.Length);
            if (input == 0)
            {
                DisplayInventory(inventorySlots); //인벤토리로 돌아가기
            }

            Slot selectedSlot = inventorySlots[input - 1];
            selectedSlot.IsUsed = !selectedSlot.IsUsed; // 장착/해제 상태 변경
            DisplayInventory(inventorySlots); // 변경된 상태로 다시 인벤토리 표시

        }

        // 아이템 정렬 옵션
        enum ItemSortOption
        {
            Name = 1,
            EquippedOrder,
            Attack,
            Defense,
        }

        //아이템 정렬
        static void SortDisplay(Slot[] inventorySlots)
        {
            /*
            Console.Clear();
            Console.WriteLine("인벤토리 - 아이템 정렬");
            Console.WriteLine("보유 중인 아이템을 정렬할 수 있습니다.");
            Console.WriteLine();
            Console.WriteLine("[아이템 목록]");
            DisplayInventory(inventorySlots);
            */
            Console.WriteLine("1. 이름");
            Console.WriteLine("2. 장착 순서(장착 아이템 우선 배치)");
            Console.WriteLine("3. 공격력");
            Console.WriteLine("4. 방어력");
            Console.WriteLine("0. 나가기");

            int input = CheckValidInput(0, 4);

            if (input == 0)
            {
                DisplayInventory(inventorySlots); // 인벤토리로 돌아가기
            }
            else
            {
                ItemSortOption sortOption = (ItemSortOption)input;
                SortInventory(inventorySlots, sortOption);
            }
        }

        // 아이템 정렬 메서드
        static void SortInventory(Slot[] inventorySlots, ItemSortOption option)
        {
            Slot[] sortedInventory = inventorySlots;

            switch (option)
            {
                case ItemSortOption.Name:
                    sortedInventory = sortedInventory.OrderBy(slot => slot.SlotItem.Name).ToArray();
                    break;
                case ItemSortOption.EquippedOrder:
                    // IsUsed (장착 상태)를 기준으로 내림차순으로 정렬(장착 아이템 우선)
                    sortedInventory = sortedInventory.OrderByDescending(slot => slot.IsUsed).ToArray();
                    break;
                //case ItemSortOption.EquippedOrder:
                //    sortedInventory = sortedInventory.OrderBy(slot => slot.IsUsed).ToArray();
                //    break;
                case ItemSortOption.Attack:
                    // 공격력을 기준으로 내림차순으로 정렬
                    sortedInventory = sortedInventory.OrderByDescending(slot => slot.SlotItem.Type == "무기" ? slot.SlotItem.Effect : 0).ToArray();
                    break;
                //case ItemSortOption.Attack:
                //    sortedInventory = sortedInventory.OrderBy(slot => slot.SlotItem.Type == "무기" ? slot.SlotItem.Effect : 0).ToArray();
                //    break;
                case ItemSortOption.Defense:
                    // 방어력을 기준으로 내림차순으로 정렬
                    sortedInventory = sortedInventory.OrderByDescending(slot => slot.SlotItem.Type == "방어구" ? slot.SlotItem.Effect : 0).ToArray();
                    break;
                    //case ItemSortOption.Defense:
                    //    sortedInventory = sortedInventory.OrderBy(slot => slot.SlotItem.Type == "방어구" ? slot.SlotItem.Effect : 0).ToArray();
                    //    break;
            }

            // 정렬된 결과를 inventorySlots 배열에 반영
            for (int i = 0; i < sortedInventory.Length; i++)
            {
                inventorySlots[i] = sortedInventory[i];
            }

            // 정렬된 결과로 인벤토리로 돌아감
            DisplayInventory(inventorySlots);
        }

        static void DisplayShop()
        {
            Console.Clear();
            Console.WriteLine("상점");
            Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
            Console.WriteLine();
            Console.WriteLine("[보유 골드]");
            Console.WriteLine($"{player.Gold} G");
            Console.WriteLine();
            Console.WriteLine("[아이템 목록]");

            for (int i = 0; i < items.Length; i++)
            {
                Item shopItem = items[i];
                string part = "";

                if (shopItem.Type == "무기")
                {
                    part = "공격력+";
                }
                else if (shopItem.Type == "방어구")
                {
                    part = "방어력+";
                }
                string itemInfo = $"{i + 1}.{shopItem.Name}\t|{part}{shopItem.Effect,-5}|{shopItem.Desc,-30}\t\t|{shopItem.Type}\t|";
                string purchaseStatus = IsItemPurchased(shopItem) ? "구매완료" : $"{shopItem.Price}G";
                Console.WriteLine($"- {itemInfo}{purchaseStatus}");
            }

            Console.WriteLine("1. 아이템 구매");
            Console.WriteLine("0. 나가기");

            int input = CheckValidInput(0, 1);

            switch (input)
            {
                case 0:
                    DisplayGameIntro();
                    break;
                case 1:
                    BuyItem(); // 아이템 구매 메서드 호출
                    break;
            }
        }


        static void BuyItem()
        {
            Console.Clear();
            Console.WriteLine("아이템 구매");
            Console.WriteLine("구매할 아이템을 선택하세요.");
            Console.WriteLine();
            Console.WriteLine("[아이템 목록]");

            for (int i = 0; i < items.Length; i++)
            {
                Item shopItem = items[i];
                string part = "";

                if (shopItem.Type == "무기")
                {
                    part = "공격력+";
                }
                else if (shopItem.Type == "방어구")
                {
                    part = "방어력+";
                }
                string itemInfo = $"{i + 1}.{shopItem.Name}\t|{part}{shopItem.Effect,-5}|{shopItem.Desc,-30}\t\t|{shopItem.Type}\t|";
                string purchaseStatus = IsItemPurchased(shopItem) ? "구매완료" : $"{shopItem.Price}G";
                Console.WriteLine($"- {itemInfo}{purchaseStatus}");
            }

            Console.WriteLine("0. 돌아가기");

            int input = CheckValidInput(0, items.Length);

            if (input == 0)
            {
                DisplayShop();
                return;
            }

            Item selectShopItem = items[input - 1]; //선택아이템 1번(item 배열의 0번째)~...

            if (IsItemPurchased(selectShopItem)) //구매한 아이템인경우
            {
                Console.WriteLine("이미 구매한 아이템입니다.");
            }
            else if (player.Gold < selectShopItem.Price) // 소지 골드가 부족한 경우
            {
                Console.WriteLine("골드가 부족합니다.");
            }
            else // 나머지 (미구매 상품이고 골드가 충분할 때)
            {
                // 아이템 구매
                player.Gold -= selectShopItem.Price; //골드 차감
                AddItemToInventory(selectShopItem); //아이템을 인벤토리에 추가
                Console.WriteLine($"{selectShopItem.Name}을(를) 구매했습니다."); //구매 Txt
                SaveItemAsPurchased(selectShopItem); //구매 정보 저장(다시 상점에 와도 구매가 떠있도록)
            }

            // 구매 결과 표시 후 상점 화면으로 돌아감
            Console.WriteLine("아무 키나 누르면 계속합니다...");
            Console.ReadKey();
            DisplayShop();
        }

        //상점의 아이템 구매여부 판별
        static bool IsItemPurchased(Item item)
        {

            // 이미 구매한 아이템인지 확인(purchasedItems에서 동일한 이름찾기)
            // 구매한 아이템 목록을 저장하는 방법에 따라 구현★
            return purchasedItems.Contains(item.Name);
        }


        static void SaveItemAsPurchased(Item item)
        {
            // 아이템을 구매했다는 정보를 저장
            purchasedItems.Add(item.Name);
        }

        static void AddItemToInventory(Item itemToAdd) //여기가 문제★
        {
            // 인벤토리 슬롯중 빈 슬롯에 아이템을 추가
            foreach (var slot in inventorySlots)
            {
                if (!slot.IteminSlot)
                {
                    slot.SlotItem = itemToAdd;
                    slot.IteminSlot = true;
                    break; // 아이템을 추가하면 반복문을 빠져나옵니다.
                }
            }
        }

        static int CheckValidInput(int min, int max)
        {
            while (true)
            {
                string input = Console.ReadLine();

                bool parseSuccess = int.TryParse(input, out var ret);
                if (parseSuccess)
                {
                    if (ret >= min && ret <= max)
                        return ret;
                }

                Console.WriteLine("잘못된 입력입니다.");
            }
        }

    }

    //데이터 통로(캐릭터)
    public class Character
    {
        public string Name { get; set; }
        public string Job { get; set; }
        public int Level { get; set; }
        public int Atk { get; set; }
        public int Def { get; set; }
        public int Hp { get; set; }
        public int Gold { get; set; }

        public Character(string name, string job, int level, int atk, int def, int hp, int gold)
        {
            Name = name;
            Job = job;
            Level = level;
            Atk = atk;
            Def = def;
            Hp = hp;
            Gold = gold;
        }
    }
    //데이터 통로(아이템)
    public class Item
    {
        public string Name { get; }
        public int Effect { get; }
        public string Desc { get; }
        public string Type { get; }
        public int Price { get; }

        public Item(string name, int effect, string desc, string type, int price)
        {
            Name = name;
            Effect = effect;
            Desc = desc;
            Type = type;
            Price = price;

        }

        // 아이템 식별자를 반환하는 메서드 추가
        public string PickItem()
        {
            return Name; // 아이템 이름을 식별자로 사용.
                         // 아이템 번호를 추가하고 식별자로 사용할 수 있지만 코드 가독성(가시성?) 이 떨어짐

        }
    }
}