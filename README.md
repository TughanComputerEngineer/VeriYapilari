
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
| 032390027  | Emrecan Kutlu    | Admin Panel Gösterimi - Tuğbayla ortak çalışma |
| 032390030  | Tuğba Önen       | Sonraki Fikstürler Menüsü - Gelecek maçların gösterimi |
| 032390029  | İsmail Karatay   | Ana Menü - Takımların görüntülenmesi ve skor bazlı sıralama |

---

## 📦 Kullanılan Veri Yapıları ve Tercih Nedenleri

### 1. 🔗 Linked List – Polat Ceylan

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

### 3. ⏳ Queue – Emrecan Kutlu & Tuğba Önen

**Kullanım Amacı:**  
- Fikstürdeki maçların sırayla uygulanması

**Neden Tercih Edildi:**
- `FIFO (First In First Out)` yapısı sayesinde maçların oynanma sırasına göre işlenmesini sağlar

**Zaman Karmaşıklığı:**
- Ekleme/çıkarma: `O(1)`

---

### 4. 🔺 Heap Tree (Max-Heap) – İsmail Karatay

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
| Binary Search Tree| Takım arama işlemleri                     | O(log n)  | O(log n)  | O(log n)  | O(n)      | Admin panelde hızlı erişim |
| Queue             | Maç fikstür sırası                        | O(1)      | O(1)      | -         | O(n)      | Maçlar sırayla oynanır |
| Max-Heap          | Puan sıralaması ve lider takım belirleme  | O(log n)  | O(log n)  | -         | -         | En yüksek puanlı takım anında bulunur |

---

## 📌 Sonuç

Bu projede kullanılan veri yapıları, sistemin hem *performansını artırmak* hem de *kullanıcı deneyimini geliştirmek* amacıyla özel olarak seçilmiştir.  
Karmaşık işlemler bile hızlıca işlenebilir ve kullanıcıya anında doğru bilgi sunulur.
