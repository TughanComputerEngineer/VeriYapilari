// Takım işlemleri
function getTeams() {
    return JSON.parse(localStorage.getItem('teams') || '[]');
}

function setTeams(teams) {
    localStorage.setItem('teams', JSON.stringify(teams));
}

function renderTeams() {
    const teams = getTeams();
    const tbody = document.getElementById('teamList');
    tbody.innerHTML = '';
    teams.forEach((team, i) => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td>${team.name}</td>
            <td>${team.league}</td>
            <td><img src="${team.logo || ''}" class="team-logo-table" onerror="this.src='https://via.placeholder.com/36x36?text=+';"></td>
            <td><button class="btn-red" onclick="deleteTeam(${i})">Sil</button></td>
        `;
        tbody.appendChild(tr);
    });
    renderTeamSelects();
}

function addTeam() {
    const name = document.getElementById('teamName').value.trim();
    const league = document.getElementById('teamLeague').value.trim();
    const logo = document.getElementById('teamLogoInput').dataset.url || '';
    if (!name || !league) {
        alert('Takım adı ve lig zorunlu!');
        return;
    }
    const teams = getTeams();
    teams.push({ name, league, logo });
    setTeams(teams);
    document.getElementById('teamName').value = '';
    document.getElementById('teamLeague').value = '';
    document.getElementById('teamLogoInput').value = '';
    document.getElementById('teamLogoInput').dataset.url = '';
    document.getElementById('logoPreview').innerHTML = `
        <svg width="40" height="40" fill="#444" viewBox="0 0 24 24"><path d="M12 5v14m7-7H5" stroke="#444" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg>
        <div>Takım Logosu</div>
    `;
    renderTeams();
}

function deleteTeam(idx) {
    if (!confirm('Takımı silmek istediğinize emin misiniz?')) return;
    const teams = getTeams();
    teams.splice(idx, 1);
    setTeams(teams);
    renderTeams();
    renderTeamSelects();
    renderMatches();
}

// Logo yükleme
document.getElementById('teamLogoInput').addEventListener('change', function(e) {
    const file = e.target.files[0];
    if (!file) return;
    const reader = new FileReader();
    reader.onload = function(evt) {
        document.getElementById('logoPreview').innerHTML = `<img src="${evt.target.result}" alt="Takım Logosu"><div>Takım Logosu</div>`;
        document.getElementById('teamLogoInput').dataset.url = evt.target.result;
    };
    reader.readAsDataURL(file);
});

// Takım select'lerini güncelle
function renderTeamSelects() {
    const teams = getTeams();
    const select1 = document.getElementById('matchTeam1');
    const select2 = document.getElementById('matchTeam2');
    if (!select1 || !select2) return;
    select1.innerHTML = '';
    select2.innerHTML = '';
    teams.forEach((team, i) => {
        const opt1 = document.createElement('option');
        opt1.value = i;
        opt1.textContent = team.name;
        select1.appendChild(opt1);
        const opt2 = document.createElement('option');
        opt2.value = i;
        opt2.textContent = team.name;
        select2.appendChild(opt2);
    });
}

// Maç işlemleri
function getMatches() {
    return JSON.parse(localStorage.getItem('matches') || '[]');
}

function setMatches(matches) {
    localStorage.setItem('matches', JSON.stringify(matches));
}

function renderMatches() {
    const matches = getMatches();
    const teams = getTeams();
    const tbody = document.getElementById('matchList');
    if (!tbody) return;
    tbody.innerHTML = '';
    matches.forEach((match, i) => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td>${teams[match.team1]?.name || '-'}</td>
            <td>${match.score1}</td>
            <td>${teams[match.team2]?.name || '-'}</td>
            <td>${match.score2}</td>
            <td><button class="btn-red" onclick="deleteMatch(${i})">Sil</button></td>
        `;
        tbody.appendChild(tr);
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
    const matches = getMatches();
    matches.push({ team1, team2, score1, score2 });
    setMatches(matches);
    document.getElementById('matchScore1').value = '';
    document.getElementById('matchScore2').value = '';
    renderMatches();
}

function deleteMatch(idx) {
    if (!confirm('Maçı silmek istediğinize emin misiniz?')) return;
    const matches = getMatches();
    matches.splice(idx, 1);
    setMatches(matches);
    renderMatches();
}

// Çıkış
function logout() {
    window.location.href = 'index.html';
}

// İlk yükleme
renderTeams();
renderMatches(); 