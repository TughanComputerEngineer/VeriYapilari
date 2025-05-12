
# 📊 Spor Turnuvası Puan Durumu ve Sıralama Sistemi

## 1. Proje Amacı ve Kapsamı
Bu proje, **Spor Turnuvası Puan Durumu ve Sıralama Sistemi**ni geliştirmeyi amaçlamaktadır.  
Sistem, bir lig veya turnuva kapsamında takımların maç sonuçlarına göre puanlarının **dinamik olarak güncellenmesi ve sıralanması** işlevini sağlar.

- **Kullanıcı paneli** üzerinden favori takımlar takip edilebilir.  
- **Admin paneli** ile takımların yönetimi yapılabilir.
- **Heap veri yapısı**, en yüksek puanlı takımın anlık bulunmasını sağlar.

---

## 2. Grup Üyeleri ve Sorumluluklar

| Öğrenci No | İsim           | Sorumluluklar                                 |
|------------|----------------|-----------------------------------------------|
| 032390025  | Polat Ceylan   | Kullanıcı paneli ve üyelik oluşturma          |
| 032390026  | Tuğhan Kovucu  | Admin paneli, takım ve üye yönetimi           |
| 032390027  | Emrecan Kutlu  | Sonraki fikstürlerin gösterimi                |
| 032390029  | İsmail Karatay | Maçların skor bazlı sıralanması               |
| 032390030  | Tuğba Önen     | Sonraki fikstürlerin gösterimi                |

---

## 3. Kullanılan Veri Yapıları ve Amaçları

### 🔗 Linked List – Polat Ceylan
- **Kullanım:** Favori takımların sıralı olarak gösterimi.
- **Avantaj:** Dinamik boyut, hızlı ekleme/silme (O(1)).
- **Neden?** Takımların sıralı listelenmesi için idealdir.

---

### 🌲 Binary Search Tree – Tuğhan Kovucu
- **Kullanım:** Takımların hızlıca aranması.
- **Avantaj:** Ortalama arama süresi O(log n).
- **Neden?** Yüzlerce takım arasında arama etkinliğini artırır.

---

### 📋 Queue – Emrecan Kutlu & Tuğba Önen
- **Kullanım:** Fikstür maçlarının sıralı gösterimi.
- **Avantaj:** FIFO prensibi ile O(1) ekleme/çıkarma.
- **Neden?** Maçların oynanma sırasına göre işlenmesini sağlar.

---

### 🔺 Heap Tree – İsmail Karatay
- **Kullanım:** Skorlara göre sıralama ve lider takımın belirlenmesi.
- **Avantaj:** En yüksek puanı O(1)'de bulma, O(log n)'de güncelleme.
- **Neden?** Lider takımın anlık bulunması için idealdir.

---

## 4. Veri Yapıları - Zaman Karmaşıklığı Özeti

| Veri Yapısı     | Ekleme     | Arama      | Silme      | Kullanım Alanı                          |
|------------------|------------|------------|------------|-----------------------------------------|
| Linked List      | O(1)       | O(n)       | O(1)       | Favori takımlar                         |
| Binary Search Tree | O(log n) | O(log n)   | O(log n)   | Takım aramaları                         |
| Queue            | O(1)       | -          | O(1)       | Maç fikstür sıralaması                  |
| Heap Tree        | O(log n)   | O(1)       | O(log n)   | En yüksek puanlı takım sıralaması       |

---

## 5. Sonuç

Bu proje:
- Verimli veri yapıları ile **dinamik**, **hızlı** ve **esnek** bir sıralama sistemi sunar.
- **Kullanıcı ve admin** deneyimi optimize edilmiştir.
- Performans odaklı yapılar sayesinde yüksek **verimlilik** sağlar.

> Projede kullanılan veri yapıları:
> - `Heap`
> - `Linked List`
> - `Binary Search Tree`
> - `Queue`

Her biri, uygulamanın ihtiyaçlarına yönelik olarak **doğru yerde** kullanılmıştır.
