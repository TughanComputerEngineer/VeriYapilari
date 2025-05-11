document.addEventListener("DOMContentLoaded", () => {
  const takimListesi = document.getElementById("takim-listesi");
  const favoriListesi = document.getElementById("favori-listesi");
  const searchInput = document.querySelector(".search-bar input");

  const username = document.body.dataset.username;
  let tumTakimlar = [];
  let favoriTakimlar = [];

  
  fetch("/api/takimlar")
    .then(res => res.json())
    .then(data => {
      tumTakimlar = data;
      takimlariYukle(tumTakimlar); 
      if (username) {
        favorileriCek(); 
      }
    });

  function favorileriCek() {
    fetch("/api/favoriler/listele")
      .then(res => res.status === 401 ? [] : res.json())
      .then(data => {
        favoriTakimlar = data;
        favoriListesiGuncelle();
        takimlariYukle(tumTakimlar); 
      });
  }

  function takimlariYukle(takimlar) {
    takimListesi.innerHTML = "";
    takimlar.forEach((takim, index) => {
      const favorideMi = favoriTakimlar.some(f => f.id === takim.id);
      const row = document.createElement("div");
      row.className = "team-row";
      row.innerHTML = `
        <span>${index + 1}</span>
        <span>${takim.isim}</span>
        <span>${takim.son5mac.map(m => `<span class="${macRenk(m)}">${m}</span>`).join("")}</span>
        <span>${takim.puan}</span>
        <span>
          <img src="images/${favorideMi ? "fullstar.png" : "emptystar-2.png"}"
               class="star-icon"
               data-teamid="${takim.id}" />
        </span>
      `;
      takimListesi.appendChild(row);
    });
  }

  function favoriListesiGuncelle() {
    favoriListesi.innerHTML = "";
    if (favoriTakimlar.length === 0) {
      favoriListesi.innerHTML = "<p>Henüz favori eklemediniz.</p>";
      return;
    }

    favoriTakimlar.forEach(team => {
      const takimIndex = tumTakimlar.findIndex(t => t.id === team.id);
      const sira = takimIndex + 1;

      const box = document.createElement("div");
      box.className = "favorite-team";
      box.innerHTML = `
        <span>
          ${sira}. ${team.isim} — Puan: ${team.puan}
          <img src="images/fullstar.png" class="star-icon" data-teamid="${team.id}" style="margin-left: 8px;" />
        </span>
      `;
      favoriListesi.appendChild(box);
    });
  }

  
  document.addEventListener("click", e => {
    if (e.target.classList.contains("star-icon")) {
      if (!username) {
        alert("Favori eklemek için giriş yapmalısınız.");
        return;
      }

      const teamId = parseInt(e.target.dataset.teamid);
      fetch(`/home/togglefavorite?teamId=${teamId}`, {
        method: "POST"
      }).then(() => favorileriCek());
    }
  });

  function macRenk(harf) {
    switch (harf) {
      case "W": return "win";
      case "L": return "loss";
      case "D": return "draw";
      default: return "";
    }
  }

  //  Arama kutusu
  searchInput?.addEventListener("input", () => {
    const q = searchInput.value.toLowerCase();
    const filtre = tumTakimlar.filter(t => t.isim.toLowerCase().includes(q));
    takimlariYukle(filtre);
  });
});
