// Takım işlemleri
function getTeams() {
    return fetch("/api/takimlar")
        .then(res => res.json());
}

function setTeams(teams) {
    localStorage.setItem('teams', JSON.stringify(teams));
}

function renderTeams() {
    getTeams().then(teams => {
        const tbody = document.getElementById('teamList');
        tbody.innerHTML = '';
        teams.forEach((team, i) => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${team.isim}</td>
                <td>${team.puan !== undefined && team.puan !== null ? team.puan : '-'}</td>
                <td><img src="${team.logo}" class="team-logo-table" onerror="this.src='/images/team_logos/placeholder.png';"></td>
                <td><button class="btn-red" onclick="deleteTeam(${team.id})">Sil</button></td>
            `;
            tbody.appendChild(tr);
        });
    });
}

function addTeam() {
    const nameElement = document.getElementById('teamName');
    const logoElement = document.getElementById('teamLogoInput');

    // Eğer öğe null ise, hata mesajı ver
    if (!nameElement || !logoElement) {
        alert("Eksik öğe var! Lütfen tüm alanları doldurduğunuzdan emin olun.");
        return;
    }

    const name = nameElement.value.trim();
    //const logo
    let logo = logoElement.dataset.url?.trim();
    // Eğer logo boşsa placeholder ata
    if (!name) {
        alert('Takım adı zorunludur!');
        return;
    }
    if (!logo) {
        logo = `/images/team_logos/${name.toLowerCase().replace(/\s+/g, '')}.png`;
    }

    // Takım bilgilerini POST ile ekle
    fetch('/admin/add-team', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ name, logo })
    })
    .then(response => response.json())
    .then(result => {
        if (result.success) {
            // Başarıyla ekledikten sonra formu sıfırla
            nameElement.value = '';
            logoElement.value = '';
            logoElement.dataset.url = ''; // logo URL'sini temizle
            document.getElementById('logoPreview').innerHTML = `
                <svg width="40" height="40" fill="#444" viewBox="0 0 24 24">
                    <path d="M12 5v14m7-7H5" stroke="#444" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                </svg>
                <div>Takım Logosu</div>
            `;
            renderTeams(); // listeyi güncelle
            renderTeamSelects();
        } else {
            if(result.message) {
                alert(result.message);
            }
            else 
            {
                alert('Takım eklenirken bir hata oluştu.');
            }

        }
    })
    .catch(err => {
        console.error(err);
        alert('Sunucuya bağlanırken hata oluştu.');
    })
}

function deleteTeam(id) {
    fetch(`/admin/delete-team`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ id }) // gönderilen veri
    })
    .then(response => {
        if (!response.ok) throw new Error('Sunucu hatası');
        return response.json();
    })
    .then(result => {
        if (result.success) {
            renderTeams(); // listeyi güncelle
            renderTeamSelects();
            renderMatches();
        } else {
            alert('Silme işlemi başarısız.');
        }
    })
}



// Takım select'lerini güncelle
async function renderTeamSelects() {
    const teams = await getTeams();
    const select1 = document.getElementById('matchTeam1');
    const select2 = document.getElementById('matchTeam2');
    if (!select1 || !select2) return;
    select1.innerHTML = '';
    select2.innerHTML = '';
    teams.forEach((team) => {
        const opt1 = document.createElement('option');
        opt1.value = team.id;
        opt1.textContent = team.isim;
        select1.appendChild(opt1);

        const opt2 = document.createElement('option');
        opt2.value = team.id;
        opt2.textContent = team.isim;
        select2.appendChild(opt2);
    });
}

// Maç işlemleri
function getMatches() {
    return fetch("/api/match")
        .then(res => res.json())  // API'den JSON verisini al
        .then(matches => {
            return matches;  // Çekilen maçları döndür
        })
        .catch(error => {
            console.error('Hata:', error);
            return [];  // Hata durumunda boş bir dizi döndür
        });
}

async function renderMatches() {
    const matches = await getMatches();
    const teams = await getTeams();
    const tbody = document.getElementById('matchList');
    if (!tbody) return;
    tbody.innerHTML = '';
    matches.forEach((match, i) => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td>${match.team1Name || '-'}</td> <!-- Takım 1 adı burada alınacak -->
            <td>${match.score1}</td>
            <td>${match.team2Name || '-'}</td> <!-- Takım 2 adı burada alınacak -->
            <td>${match.score2}</td>
            <td><button class="btn-red" onclick="deleteMatch(${match.id})">Sil</button></td>
        `;
        tbody.appendChild(tr);
    });
}

function deleteMatch(matchId) {
    fetch('/admin/delete-match', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ id: matchId })
    })
    .then(res => res.json())
    .then(data => {
        if (data.success) {
            renderMatches(); // listeyi yenile
        } else {
            alert(data.message || 'Silme işlemi başarısız oldu.');
        }
    })
    .catch(err => {
        console.error(err);
        alert('Bir hata oluştu.');
    });
}

function addMatch() {
    const team1 = parseInt(document.getElementById('matchTeam1').value);
    const team2 = parseInt(document.getElementById('matchTeam2').value);
    const score1 = parseInt(document.getElementById('matchScore1').value);
    const score2 = parseInt(document.getElementById('matchScore2').value);

    if (isNaN(team1) || isNaN(team2) || team1 === team2) {
        alert('Farklı iki takım seçmelisiniz!');
        return;
    }
    if (isNaN(score1) || isNaN(score2)) {
        alert('Skorlar boş olamaz!');
        return;
    }

    fetch('/admin/add-match', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ team1Id: team1, team2Id: team2, score1, score2 })
    })
    .then(res => res.json())
    .then(data => {
        if (data.success) {
            document.getElementById('matchScore1').value = '';
            document.getElementById('matchScore2').value = '';
            renderMatches();
        } else {
            alert(data.message);
        }
    })
    .catch(err => {
        console.error(err);
        alert('Bir hata oluştu.');
    });
}

// çıkış
function logout() {
    fetch('/user/logout', {
        method: 'POST'
    })
    .then(res => {
        if (res.redirected) {
            window.location.href = res.url;  // Backend'in yönlendirdiği sayfaya git
        } else {
            return res.json();
        }
    })
    .catch(err => {
        console.error(err);
        alert('Çıkış yapılırken bir hata oluştu.');
    });
}

// İlk yükleme
renderTeams();
renderMatches(); 
renderTeamSelects();