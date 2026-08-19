using BalancingApi.Services;

namespace BalancingApi.Tests;

public class BalancingTest
{
    public static void RunAllTests()
    {
        var service = new BalancingService();

        Console.WriteLine("Mulai testing Case 1..");

        var res1 = service.Balance([4, 5, 1, 7, 6, 4, 0]);
        CekHasil("sample Case", res1, [4, 5, 4, 5, 5, 4, 0], [4, 5, 1, 7, 6, 4, 0]);

        var res2 = service.Balance([6, 3, 3, 0]);
        CekHasil("total Habis Dibagi", res2, [4, 4, 4, 0], [6, 3, 3, 0]);

        var res3 = service.Balance([5, 2, 0]);
        CekHasil("total Bersisa", res3, [4, 3, 0], [5, 2, 0]);

        var res4 = service.Balance([0, 0, 0, 0]);
        CekHasil("semua Nol", res4, [0, 0, 0, 0], [0, 0, 0, 0]);

        var res5 = service.Balance([0, 10, 0, 0]);
        CekHasil("satu Aktif", res5, [0, 10, 0, 0], [0, 10, 0, 0]);

        var res6 = service.Balance([5, 5, 5, 0]);
        CekHasil("tie (Awal Sama)", res6, [5, 5, 5, 0], [5, 5, 5, 0]);

        try 
        {
            service.Balance([4, -1, 5]);
            throw new Exception("Gagal, test input negatif harusnya nangkep error!");
        } 
        catch (ArgumentException) 
        {
            Console.WriteLine("[OK] Input Negatif");
        }

        try 
        {
            service.Balance(null!);
            throw new Exception("Gagal, test input null harusnya nangkep error!");
        } 
        catch (ArgumentNullException) 
        {
            Console.WriteLine("[OK] Input Null");
        }

        // Edge cases
        var res8 = service.Balance([]);
        CekHasil("array Kosong", res8, [], []);

        var res9 = service.Balance([1000000, 2000000, 0]);
        CekHasil("angka Gede", res9, [1500000, 1500000, 0], [1000000, 2000000, 0]);

        var res10 = service.Balance([2, 2, 2, 0]);
        CekHasil("tie Dengan Sisa", res10, [2, 2, 2, 0], [2, 2, 2, 0]);

        Console.WriteLine("semua test selesai!");
    }

    private static void CekHasil(string namaTest, int[] hasil, int[] ekspektasi, int[] inputAwal)
    {
        if (!hasil.SequenceEqual(ekspektasi))
        {
            throw new Exception($"Test '{namaTest}' gagal. hasil ga sesuai ekspektasi.");
        }

        // verifikasi invariant
        if (inputAwal.Sum() != hasil.Sum()) 
        {
            throw new Exception($"Test '{namaTest}' gagal: total berubah!");
        }

        for (int i = 0; i < inputAwal.Length; i++) 
        {
            if (inputAwal[i] == 0 && hasil[i] != 0) 
            {
                throw new Exception($"'{namaTest}' gagal: slot 0 keisi.");
            }
        }

        var hariAktif = hasil.Where(x => x > 0).ToList();
        if (hariAktif.Any()) 
        {
            if (hariAktif.Max() - hariAktif.Min() > 1) 
            {
                throw new Exception($"'{namaTest}' gagal: selisih > 1.");
            }
        }

        Console.WriteLine($"[OK] {namaTest}");
    }
}