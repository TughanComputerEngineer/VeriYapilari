Projenin github linki: https://github.com/TughanComputerEngineer/VeriYapilari

# 🏆 Spor Turnuvası Puan Durumu ve Sıralama Sistemi

## 🎯 Proje Amacı ve Kapsamı

Bu proje, bir spor ligi ya da turnuvası kapsamında *takımların maç sonuçlarına göre puanlarının güncellenip sıralandığı* bir sistemin geliştirilmesini amaçlamaktadır.

Sistemin temel hedefleri:

- Maç sonuçlarına göre puan durumunu *dinamik olarak takip edebilmek*
- Kullanıcıya *kolay bir arayüz* sunmak
- *En yüksek puanlı takımı anlık olarak* belirleyebilmek

> Performans odaklı olarak çeşitli *veri yapıları* kullanılmış, özellikle maksimum puanı hızlıca bulmak için *Heap veri yapısı* tercih edilmiştir.

---

## 👨‍💻 Grup Üyeleri ve Görev Dağılımı

| Öğrenci No | Ad Soyad        | Görevi |
|------------|------------------|--------|
| 032390025  | Polat Ceylan     | Lig Tablosu - Favori takımların yönetilmesi |
| 032390026  | Tuğhan Kovucu    | Admin Panel İşlemleri - Takım bilgilerinin yönetimi ve arama işlemleri |
| 032390027  | Emrecan Kutlu    | Admin Panel Gösterimi - İsmail'le ortak çalışma |
| 032390030  | Tuğba Önen       | Sonraki Fikstürler Menüsü - Polat'la ortak çalışma |
| 032390029  | İsmail Karatay   | Ana Menü - Takımların görüntülenmesi ve skor bazlı sıralama |

---

## 📦 Kullanılan Veri Yapıları ve Tercih Nedenleri

### 1. 🔗 Linked List – Polat Ceylan & Tuğba Önen

**Kullanım Amacı:**  
- Favori takımların ve genel takım listesinin sıralı şekilde yazdırılması

**Neden Tercih Edildi:**
- Elemanların sırayla gezilmesi ve yazdırılması gereken durumlar için idealdir
- Yeni takım eklemelerinde yeniden sıralamaya gerek kalmaz

**Zaman Karmaşıklığı:**
- Ekleme (başa): `O(1)`
- Listeleme: `O(n)`

---

### 2. 🌲 Binary Search Tree (BST) – Tuğhan Kovucu

**Kullanım Amacı:**  
- Takım aramalarının hızlı gerçekleştirilmesi

**Neden Tercih Edildi:**
- Ortalama arama süresi: `O(log n)`
- Düzenli veri tutumu ve hızlı erişim sağlar

---

### 4. 🔺 Heap Tree (Max-Heap) – İsmail Karatay & Emrecan Kutlu

**Kullanım Amacı:**  
- Takımların puanına göre sıralanması ve lider takımın hızlıca bulunması

**Neden Tercih Edildi:**
- `Max-Heap` yapısı, en yüksek puanlı takımı kökte tutar
- Maç sonuçlarına göre heap güncellenir

**Zaman Karmaşıklığı:**
- En yüksek değeri alma: `O(1)`
- Ekleme/silme: `O(log n)`

---

## ➕ 4. Veri Yapıları - Zaman Karmaşıklığı Özeti ve Kullanım Yerleri

| Veri Yapısı      | Kullanım Yeri                              | Ekleme    | Silme     | Arama     | Listeleme | Açıklama |
|------------------|---------------------------------------------|-----------|-----------|-----------|-----------|----------|
| Linked List       | Takım listesi, favoriler                  | O(1)      | O(n)      | O(n)      | O(n)      | Sıralı yazdırma için kullanılır |
| Binary Search Tree| Takım arama işlemleri                     | O(log n)  | O(log n)  | O(log n)  | O(n)      | Ana menüdeki takımlara hızlı erişim |
| Max-Heap          | Puan sıralaması ve lider takım belirleme  | O(log n)  | O(log n)  | -         | -         | En yüksek puanlı takım anında bulunur |

---

## 📌 Sonuç

Bu proje, verimli veri yapıları kullanılarak tasarlanmış, dinamik ve hızlı bir sıralama sistemi sunar. Kullanıcılar ve adminler için kullanıcı dostu bir deneyim sağlanırken, sistemin performansı her veri işlemi için optimize edilmiştir. Heap, Linked List ve Binary Search Tree gibi veri yapıları, sistemin hem esnek hem de performanslı olmasını sağlar.
