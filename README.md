# Balancing Production API

Track: Back-end Only (.NET Web API)

## Case 1 - solusi & algoritma Balancing

### algoritma

logika balancing ini tujuannya buat meratakan angka produksi tanpa ngubah total hasil dan tetep ngejaga hari libur (angka 0) tetep 0.

Langkah langkahnya:

1. validasi input, kalau null atau ada angka negatif langsung throw error.
2. kumpulin index dari slot yang nilainya > 0 (slot aktif). kalau ga ada slot aktif atau array kosong, langsung balikin salinan array asli.
3. hitung rata-rata dasar (total / jumlahSlotAktif) sama sisa pembagiannya (total % jumlahSlotAktif).
4. buat bagi bonus sisa (+1), slot aktif diurutin berdasarkan nilai awal paling gede. Kalau nilainya sama, diurutin dari index terawal. Terus ambil sebanyak `sisa` slot teratas.
5. bikin array hasil baru, isi nilai rata-rata (+1 buat slot yang dapet bonus). Slot bernilai 0 tetep dibiarin 0.

## kompleksitas

- time Complexity: O(N log N) -> karena ada proses sorting slot aktif.
- space Complexity: O(N) -> butuh memory tambahan buat nyimpen list index aktif dan array hasil.

## asumsi business rules

- kalau inputnya array kosong, outputnya tetep array kosong.
- kalau semua slot nilainya 0, balikin copy array asli tanpa diutak-atik.
- fungsi bersifat pure function (ga ngerubah array input asli).

## edge cases yang dites

1. array kosong : spaya ga kena error pembagian nol (DivideByZeroException).
2. angka rencana awal gede: supaya ga kena Integer Overflow pas ngitung total sum.
3. tie dengan sisa: supaya urutan pembagian sisa tetep konsisten dan ga ngacak pas ada nilai awal yang sama.

## Case 2 - Application & Persistence Architecture

## Pemodelan Data

1. Struktur data dipisah menjadi 2 entity dengan relasi one to many:

2. Planning (Header): Menyimpan metadata transaksi seperti PlanningId, RequestCode (Unique), CandidateToken, CreatedAt, dan Status.

3. PlanningSlot (Detail): Menyimpan rincian slot produksi seperti SlotOrder, SlotName, OriginalQuantity, BalancedQuantity, dan IsActive.

Alasan Pemodelan: Pemisahan ini mempermudah pencarian histori transaksi header tanpa memuat seluruh slot data jika tidak diperlukan, serta mendukung jumlah slot yang dinamis.

## Environment & Versi Runtime

- **Runtime / SDK:** .NET 8/10 SDK
- **Database:** PostgreSQL (Lokal atau Docker)
- **Framework:** Entity Framework Core (Npgsql Provider)

## Cara Menjalankan Aplikasi

1. Persiapkan Database PostgreSQL  
   Pastikan service PostgreSQL di komputer sudah berjalan.

2. Konfigurasi Connection String  
   Buka file `appsettings.sample.json` dan sesuaikan koneksi PostgreSQL lokal, contoh:

"ConnectionStrings": {
"DefaultConnection": "Host=localhost;Port=5432;Database=balancing_db;Username=postgres;Password=postgres"
}

3. rename file `appsettings.sample.json` jadi file `appsettings.json`
4. selanjutnya `dotnet build terlebih dahulu`
5. jalankan 'dotnet run' pada powershell/cmd, sedangakan untuk menjalankan unit testing API 'dotnet run -- --run-tests'

## Referensi & Dokumentasi Pendukung

Pengerjaan Case 1 mengacu pada dokumentasi resmi Microsoft C# / .NET untuk manipulasi array, LINQ, serta dasar-dasar pembuatan automated testing:

- Testing Concepts & Unit Testing in .NET  
  [https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)  
  Referensi utama untuk belajar dasar automated testing dan alur penulisan assertion di .NET.

- Exception Handling in C# (try-catch)  
  [https://stackoverflow.com/questions/14973642/how-using-try-catch-for-exception-handling-is-best-practice](https://stackoverflow.com/questions/14973642/how-using-try-catch-for-exception-handling-is-best-practice)  
  Digunakan untuk mempelajari cara menangkap exception dan menguji validasi input tidak valid pada test.

- LINQ Ordering (OrderByDescending & ThenBy)  
  [https://stackoverflow.com/questions/3453278/linq-orderbydescending-thenbydescending-issue](https://stackoverflow.com/questions/3453278/linq-orderbydescending-thenbydescending-issue)  
  Digunakan untuk mengurutkan slot aktif berdasarkan nilai awal terbesar dan prioritas indeks terawal.

- Enumerable.SequenceEqual  
  [https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.sequenceequal](https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.sequenceequal)  
  Digunakan pada unit test untuk membandingkan kesesuaian isi dua array.

- Array.Copy Method  
  [https://learn.microsoft.com/en-us/dotnet/api/system.array.copy](https://learn.microsoft.com/en-us/dotnet/api/system.array.copy)  
  Digunakan untuk duplikasi array secara aman tanpa merubah input asli.

- Entity Framework Core PostgreSQL Provider (Npgsql):
  [https://learn.microsoft.com/en-us/ef/core/providers/npgsql/](https://learn.microsoft.com/en-us/ef/core/providers/npgsql/)
  setup postgre pada entity framework.

- Flow diagram aplikasi.
<p align="center">
  <img 
    src="https://i.ibb.co.com/5hWSf4hG/API-Request-Handling-2026-08-20-004241.png" 
    width="300"
    alt="API Request Handling"
  >
</p>

- Design Pattern aplikasi.

<p align="center">
  <img 
    src="https://i.ibb.co.com/8gDmh3KM/API-Request-Handling-2026-08-20-005914.png"
    width="600"
    alt="API Request Handling"
  >
</p>

- Database design.

<p align="center">
  <img 
    src="https://i.ibb.co.com/CpW9xGfz/employee-public-payment.png"
    width="400"
    alt="API Request Handling"
  >
</p>

## Query

FileQuery  
 [https://drive.google.com/file/d/1pQSzvI0IWPeLmHURGMqIHcM2uSFlDcmL/view?usp=sharing](https://drive.google.com/file/d/1pQSzvI0IWPeLmHURGMqIHcM2uSFlDcmL/view?usp=sharing)
