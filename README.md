# Procedural Low Poly Island Generator

Unity Version: 6000.4.6f1

A fully customizable, procedural low-poly island generator for Unity Universal Render Pipeline - URP. This tool generates stylized, voxel or low-poly terrain chunks with true flat-shading, and utilizes a Voronoi-based biome distribution system, the Weighted Deck algorithm, to create distinct regional biomes.

Türkçe versiyon aşağıdadır

## Key Features

* **Voronoi-Based Regional Biomes:** Instead of noisy, patchy biomes, this system divides the island into massive, contiguous regions similar to Valheim.
* **Weighted Biome Spawning:** Uses a mathematical **Deck/Pool** system ensuring exact representation based on user-defined spawn weights. Guaranteed minimum 1 spawn per biome provided the region count allows it.
* **True Flat Shading - Blocky Borders:** Triangles are processed per-quad to prevent ugly **sawtooth** diagonal color bleeding at biome borders, resulting in clean, Minecraft-like pixelated edges.
* **Island Falloff:** A mathematical radial falloff mask ensures the terrain always naturally slopes down into the ocean at the edges.
* **Vertex Spacing Scaling:** Scale the physical size of the triangles without changing grid density. Increase this, for example 3 to 5, to make the triangles massive for a stylized look, perfect for achieving that chunky BotW/Zelda low-poly aesthetic.
* **Runtime Regeneration:** Easily link a UI button to the `Button_RegenerateMap()` method to randomize the noise offset seed and generate a completely new topology instantly.

## Settings & Configuration

Attach `MapGenerator.cs` to an empty GameObject.

### 1. Chunk Settings

* **Grid Size X/Z:** How many chunks to generate.
* **Chunk Size:** Grid size per chunk, such as 20x20 vertices.
* **Vertex Spacing:** The physical distance between vertices. Increase this to make the triangles massive for a stylized look.

### 2. Visual Settings

* **Terrain Material:** Assign a standard URP Lit material. Enable Vertex Color in the shader if using a custom one.
* **Use Flat Shading:** If true, separates vertices to give hard edges for a Low Poly style.

### 3. Noise Settings

* **Noise Scale:** The zoom level of the Perlin Noise.
* **Height Multiplier:** The maximum physical height of mountains.
* **Noise Offset:** The seed. Modifying this generates a completely different physical terrain shape.

### 4. Falloff Settings

* **Use Falloff:** Keeps the terrain as an island.
* **Falloff A & B:** Curves the slope of the coastline.

### 5. Region Settings

* **Region Count:** How many Voronoi core points to spawn. Must be `>=` the amount of biomes you add.
* **Boundary Noise Scale/Amount:** Warps the borders of the biomes using Perlin Noise so they look natural instead of straight lines.

### 6. Available Biomes

* Add your custom biomes here.
* Set **Name**, **Color**, and **Spawn Weight**.
* The system guarantees exact percentage-based distribution out of the `Region Count`.

---

---

# Prosedürel Low Poly Ada Oluşturucu

Unity URP için tamamen özelleştirilebilir, prosedürel bir low-poly ada oluşturma sistemi. Bu araç, gerçek **Flat Shading** kullanan stilize araziler ve terrain chunk'ları üretir. Belirgin bölgesel biyomlar oluşturmak için Voronoi tabanlı Ağırlıklı Deste algoritması dağıtım sistemi kullanır.

## Temel Özellikler

* **Voronoi Tabanlı Bölgesel Biyomlar:** Dağınık ve noktasal biyomlar yerine, adayı Valheim tarzı devasa ve tek parça bölgelere ayırır.
* **Ağırlıklı Biyom Çıkma Oranı - Weighted Spawning:** Matematiksel **Deste/Havuz** sistemi kullanır. Girdiğiniz **Spawn Weight** yüzdelerine tam olarak uyan neticeler verir. Eklenen her biyomun haritada en az 1 kez çıkması garanti altındadır.
* **Gerçek Flat Shading - Kare Sınırlar:** Üçgenler **Quad**, yani kare bazında boyanır. Bu sayede biyom sınırlarında oluşan çirkin zikzaklı **testere dişi** renk geçişleri önlenir ve Minecraft tarzı temiz piksel/kare sınırlar elde edilir.
* **Ada Maskesi - Falloff:** Matematiksel bir dairesel maske, arazinin kenarlara doğru her zaman denize ve kumsala batmasını sağlar.
* **Vertex Spacing - Üçgen Büyüklüğü:** Izgara yoğunluğunu değiştirmeden üçgenlerin fiziksel boyutunu büyütmenizi sağlar. Klasik Zelda veya Low Poly estetiğini yakalamak için idealdir.
* **Oyun İçi Yenileme - Runtime Generation:** Arayüz üzerindeki bir butonu `Button_RegenerateMap()` fonksiyonuna bağlayarak tek tıkla noise offset gürültü değerini değiştirebilir ve yepyeni topolojilere sahip adalar üretebilirsiniz.

## Ayarlar ve Kullanım

Boş bir GameObject oluşturup içine `MapGenerator.cs` dosyasını sürükleyin.

### 1. Chunk - Parça Ayarları

* **Grid Size X/Z:** Adanın kaç adet parçadan oluşacağı.
* **Chunk Size:** Her bir parçanın içindeki ızgara sayısı, örneğin 20x20.
* **Vertex Spacing:** Noktalar arası fiziksel mesafe. Keskin Low Poly tarzı için bu değeri 3 ile 5 arasına çekebilirsiniz.

### 2. Visual - Görsellik Ayarları

* **Terrain Material:** Standart bir URP Lit materyali atayın.
* **Use Flat Shading:** Düşük poligonlu keskin hatlar için aktif kalmalıdır.

### 3. Noise - Gürültü ve Yükseklik Ayarları

* **Noise Scale:** Perlin gürültüsünün, yani dağların ve vadilerin yakınlaştırma seviyesi.
* **Height Multiplier:** Dağların maksimum yüksekliği.
* **Noise Offset:** Tohum - Seed değeri. Bu değiştirildiğinde adanın tüm fiziksel dağ ve vadi yapısı değişir.

### 4. Falloff - Ada Sınırı Ayarları

* **Use Falloff:** Haritayı bir ada şeklinde okyanusun ortasında tutar.
* **Falloff A & B:** Kıyı şeridinin suya batış eğimini ve keskinliğini ayarlar.

### 5. Region - Bölge ve Voronoi Ayarları

* **Region Count:** Haritaya kaç adet biyom çekirdeği atılacağı. Listenizdeki biyom çeşidinden büyük veya eşit olmalıdır.
* **Boundary Noise Scale/Amount:** Biyomların sınır çizgilerinin cetvelle çizilmiş gibi düz olmaması için sınırları dalgalandırır ve doğal bir girinti çıkıntı sağlar.

### 6. Available Biomes - Mevcut Biyomlar

* Kendi biyomlarınızı buradan ekleyin.
* **Name** - İsim, **Color** - Renk ve **Spawn Weight** - Çıkma Ağırlığı belirleyin.
* Sistem, verdiğiniz ağırlıkları `Region Count` üzerinden tam bir yüzde hesabına dökerek dağıtır.
