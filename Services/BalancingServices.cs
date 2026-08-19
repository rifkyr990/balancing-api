namespace BalancingApi.Services;

public class BalancingService
{
    public int[] Balance(int[] plan)
    {
        if (plan == null)
            throw new ArgumentNullException(nameof(plan), "Plan tidak boleh null.");

        for (int i = 0; i < plan.Length; i++)
        {
            if (plan[i] < 0)
                throw new ArgumentException("Input tidak boleh ada angka negatif.");
        }

        if (plan.Length == 0)
            return new int[0];

        int totalSum = 0;
        List<int> indexAktif = new List<int>();

        for (int i = 0; i < plan.Length; i++)
        {
            totalSum += plan[i];
            if (plan[i] > 0)
            {
                indexAktif.Add(i);
            }
        }

        if (indexAktif.Count == 0)
        {
            int[] copy = new int[plan.Length];
            Array.Copy(plan, copy, plan.Length);
            return copy;
        }

        int baseValue = totalSum / indexAktif.Count;
        int sisa = totalSum % indexAktif.Count;

        // urutkan berdasarkan nilai awal terbesar, lalu posisi terawal
        indexAktif = indexAktif
            .OrderByDescending(idx => plan[idx])
            .ThenBy(idx => idx)
            .ToList();

        int[] hasil = new int[plan.Length];

        for (int i = 0; i < indexAktif.Count; i++)
        {
            int idx = indexAktif[i];
            hasil[idx] = baseValue;
        }

        // bagi bonus sisa ke urutan prioritas teratas
        for (int i = 0; i < sisa; i++)
        {
            int idxBonus = indexAktif[i];
            hasil[idxBonus] += 1;
        }

        return hasil;
    }
}