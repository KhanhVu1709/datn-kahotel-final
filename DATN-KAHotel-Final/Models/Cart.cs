using Newtonsoft.Json;

namespace DATN_KAHotel_Final.Models
{
    public class Cart
    {
        public static readonly QlksContext db = new QlksContext();
        public static T GetObjectFromJson<T>(ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
        // 
        public static List<Item> GetCart(ISession session)
        {
            List<Item> cart = Cart.GetObjectFromJson<List<Item>>(session, "cart");
            return cart;
        }
        // 
        public static void AddItem(ISession session, int id, DateTime batDau, DateTime ketThuc)
        {
            if (Cart.GetObjectFromJson<List<Item>>(session, "cart") == null)
            {
                List<Item> cart = new List<Item>();
                Phong item = db.Phongs.Where(tbl => tbl.Id == id).FirstOrDefault();

                cart.Add(new Item { phong = item, batDau = batDau, ketThuc = ketThuc });
                session.SetString("cart", JsonConvert.SerializeObject(cart));
            }
        }
        //
        public static void CartDestroy(ISession session)
        {
            List<Item> cart = new List<Item>();
            session.SetString("cart", JsonConvert.SerializeObject(cart));
        }
        // kiểm tra sản phẩm theo id này đã tồn tại trong giỏ hàng hay chưa
        public static int isExist(ISession session, int id)
        {
            List<Item> cart = Cart.GetObjectFromJson<List<Item>>(session, "cart");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].phong.Id == id)
                {
                    return i;
                }
            }
            return -1;
        }
        //
        public static double CartTotal(ISession session, DateTime batDau, DateTime ketThuc)
        {
            List<Item> items_cart = Cart.GetCart(session);
            TimeSpan khoangCach = ketThuc - batDau;
            int soNgay = (int)khoangCach.TotalDays;
            if (items_cart != null)
            {
                double total = 0;
                foreach (var item in items_cart)
                {
                    total += Convert.ToDouble((item.phong.GiaPhong * soNgay) + (item.phong.GiaPhong * soNgay * 0.1));
                }
                return total;
            }
            else
                return 0;
        }
    }
}
