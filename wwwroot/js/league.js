function shuffleArray(array) {
    // Takımları rastgele karıştır
    for (let i = array.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [array[i], array[j]] = [array[j], array[i]];
    }
    return array;
}

function renderMatchTable(teams) {
    const tableContainer = document.getElementById("match-table");
    tableContainer.innerHTML = "";  // Önceki içeriği temizle

    // Tabloyu oluştur
    const table = document.createElement("table");
    table.classList.add("match-table");

    // Tablo başlıkları
    const headerRow = document.createElement("tr");
    headerRow.innerHTML = "<th>Takım 1</th><th>vs</th><th>Takım 2</th>";
    table.appendChild(headerRow);

    // Eşleşmeleri sırayla ekle
    for (let i = 0; i < teams.length; i += 2) {
        const row = document.createElement("tr");
        const team1 = teams[i];
        const team2 = teams[i + 1] || { name: "BAY", id: "-" };  // Tek takım kalırsa "BAY" olacak

        row.innerHTML = `
            <td>${team1.name} <span style="color: gray;">(#${team1.id})</span></td>
            <td>      VS      </td>
            <td>${team2.name} <span style="color: gray;">(#${team2.id})</span></td>
        `;
        table.appendChild(row);
    }

    // Tabloyu ekle
    tableContainer.appendChild(table);
}

// API'den takımları al
fetch('/api/tournament/teams')
    .then(response => response.json())
    .then(teams => {
        const shuffled = shuffleArray(teams);  // Takımları karıştır
        renderMatchTable(shuffled);  // Tabloyu render et
    })
    .catch(error => console.error('Hata:', error));

