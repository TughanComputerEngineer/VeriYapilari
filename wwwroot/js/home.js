document.addEventListener("DOMContentLoaded", () => {
  const searchInput = document.querySelector(".search-bar input");
  const dropdownToggle = document.querySelector(".search-toggle");
  const dropdownBody = document.getElementById("ligDropdown");
  const takimRows = Array.from(document.querySelectorAll(".team-row"));
  const takimListesi = document.getElementById("takim-listesi");

  // Arama filtreleme
  searchInput.addEventListener("input", () => {
    const aranan = searchInput.value.toLowerCase();
    takimRows.forEach(row => {
      const isim = row.querySelector("span:nth-child(2)").innerText.toLowerCase();
      row.style.display = isim.includes(aranan) ? "grid" : "none";
    });
  });

  // Favori yıldız tıklama
  document.addEventListener("click", function (e) {
    if (e.target.classList.contains("star-icon")) {
      const star = e.target;
      star.classList.toggle("favorited");
      star.src = star.classList.contains("favorited")
        ? "images/fullstar.png"
        : "images/emptystar-2.png";
    }
  });
});
