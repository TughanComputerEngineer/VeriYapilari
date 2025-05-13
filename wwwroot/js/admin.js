document.addEventListener('DOMContentLoaded', function () {
    // Logo seçme butonu
    const selectLogoBtn = document.getElementById('selectLogoBtn');
    const teamLogo = document.getElementById('teamLogo');
    const logoPreviewImg = document.getElementById('logoPreviewImg');
    const logoPlaceholder = document.querySelector('.logo-placeholder');

    if (selectLogoBtn) {
        selectLogoBtn.addEventListener('click', function () {
            teamLogo.click();
        });
    }

    if (teamLogo) {
        teamLogo.addEventListener('change', function () {
            if (this.files && this.files[0]) {
                const reader = new FileReader();

                reader.onload = function (e) {
                    logoPreviewImg.src = e.target.result;
                    logoPreviewImg.style.display = 'block';
                    logoPlaceholder.style.display = 'none';
                }

                reader.readAsDataURL(this.files[0]);
            }
        });
    }

    // Yeni takım ekleme modalını aç
    const createTeamBtn = document.getElementById('createTeamBtn');
    const teamModal = document.getElementById('teamModal');
    const closeButtons = document.querySelectorAll('.close, #cancelBtn');

    if (createTeamBtn) {
        createTeamBtn.addEventListener('click', function () {
            document.getElementById('modalTitle').textContent = 'Yeni Takım Ekle';
            document.getElementById('teamId').value = '0';
            document.getElementById('teamForm').reset();

            // Logo önizlemeyi sıfırla
            if (logoPreviewImg) {
                logoPreviewImg.style.display = 'none';
            }
            if (logoPlaceholder) {
                logoPlaceholder.style.display = 'flex';
            }

            teamModal.style.display = 'block';
        });
    }

    // Takım düzenleme modalını aç
    const editTeamBtns = document.querySelectorAll('.edit-team-btn');

    editTeamBtns.forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.stopPropagation();
            const teamId = this.getAttribute('data-team-id');

            // AJAX ile takım bilgilerini al
            fetch(`/Admin/GetTeamDetails?id=${teamId}`)
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        const team = data.team;

                        document.getElementById('modalTitle').textContent = 'Takım Düzenle';
                        document.getElementById('teamId').value = team.id;
                        document.getElementById('teamName').value = team.name;
                        document.getElementById('teamLeague').value = team.league;
                        document.getElementById('teamScore').value = team.score;

                        // Maç sonuçlarını doldur
                        const matches = team.matches;
                        for (let i = 0; i < matches.length && i < 5; i++) {
                            document.getElementById('match' + (i + 1)).value = matches[i];
                        }

                        // Logo önizlemeyi ayarla
                        if (team.imagePath) {
                            logoPreviewImg.src = team.imagePath;
                            logoPreviewImg.style.display = 'block';
                            logoPlaceholder.style.display = 'none';
                        } else {
                            logoPreviewImg.style.display = 'none';
                            logoPlaceholder.style.display = 'flex';
                        }

                        teamModal.style.display = 'block';
                    } else {
                        alert('Hata: ' + data.message);
                    }
                })
                .catch(error => {
                    console.error('Hata:', error);
                    alert('Takım bilgileri alınırken bir hata oluştu.');
                });
        });
    });

    // Modalları kapat
    closeButtons.forEach(button => {
        button.addEventListener('click', function () {
            teamModal.style.display = 'none';
            matchesModal.style.display = 'none';
            deleteModal.style.display = 'none';
            matchQueueModal.style.display = 'none';
        });
    });

    // Takım formunu gönder
    const teamForm = document.getElementById('teamForm');

    if (teamForm) {
        teamForm.addEventListener('submit', function (e) {
            e.preventDefault();

            const formData = new FormData(this);

            // Maç sonuçlarını ekle
            const matchResults = [];
            for (let i = 1; i <= 5; i++) {
                matchResults.push(document.getElementById('match' + i).value);
            }
            formData.append('matchResults', matchResults.join(','));

            // AJAX ile gönder
            fetch('/Admin/SaveTeam', {
                method: 'POST',
                body: formData
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        // Başarılı ise sayfayı yenile
                        window.location.reload();
                    } else {
                        alert('Hata: ' + data.message);
                    }
                })
                .catch(error => {
                    console.error('Hata:', error);
                    alert('Takım kaydedilirken bir hata oluştu.');
                });
        });
    }

    // Maç sonuçları düzenleme modalını aç
    const editMatchesBtns = document.querySelectorAll('.edit-matches-btn');
    const matchesModal = document.getElementById('matchesModal');

    editMatchesBtns.forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.stopPropagation();
            const teamId = this.getAttribute('data-team-id');
            const teamRow = document.querySelector(`tr[data-team-id="${teamId}"]`);
            const matchResults = teamRow.querySelectorAll('.match-result');

            document.getElementById('matchesTeamId').value = teamId;

            // Mevcut maç sonuçlarını doldur
            for (let i = 0; i < matchResults.length && i < 5; i++) {
                document.getElementById('editMatch' + (i + 1)).value = matchResults[i].textContent;
            }

            matchesModal.style.display = 'block';
        });
    });

    // Maç sonuçları formunu gönder
    const matchesForm = document.getElementById('matchesForm');

    if (matchesForm) {
        matchesForm.addEventListener('submit', function (e) {
            e.preventDefault();

            const teamId = document.getElementById('matchesTeamId').value;
            const matchResults = [];

            for (let i = 1; i <= 5; i++) {
                matchResults.push(document.getElementById('editMatch' + i).value);
            }

            // AJAX ile gönder
            fetch('/Admin/UpdateMatchResults', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    teamId: teamId,
                    matchResults: matchResults.join(',')
                })
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        // Başarılı ise sayfayı yenile
                        window.location.reload();
                    } else {
                        alert('Hata: ' + data.message);
                    }
                })
                .catch(error => {
                    console.error('Hata:', error);
                    alert('Maç sonuçları güncellenirken bir hata oluştu.');
                });
        });
    }

    // Takım silme modalını aç
    const deleteTeamBtns = document.querySelectorAll('.delete-team-btn');
    const deleteModal = document.getElementById('deleteModal');

    deleteTeamBtns.forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.stopPropagation();
            const teamId = this.getAttribute('data-team-id');

            document.getElementById('deleteTeamId').value = teamId;
            deleteModal.style.display = 'block';
        });
    });

    // Takım silme işlemini onayla
    const confirmDeleteBtn = document.getElementById('confirmDeleteBtn');

    if (confirmDeleteBtn) {
        confirmDeleteBtn.addEventListener('click', function () {
            const teamId = document.getElementById('deleteTeamId').value;

            // AJAX ile sil
            fetch('/Admin/DeleteTeam', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    id: teamId
                })
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        // Başarılı ise sayfayı yenile
                        window.location.reload();
                    } else {
                        alert('Hata: ' + data.message);
                    }
                })
                .catch(error => {
                    console.error('Hata:', error);
                    alert('Takım silinirken bir hata oluştu.');
                });
        });
    }

    // Sayfa dışına tıklandığında modalları kapat
    window.addEventListener('click', function (e) {
        if (e.target === teamModal) {
            teamModal.style.display = 'none';
        }
        if (e.target === matchesModal) {
            matchesModal.style.display = 'none';
        }
        if (e.target === deleteModal) {
            deleteModal.style.display = 'none';
        }
        if (e.target === matchQueueModal) {
            matchQueueModal.style.display = 'none';
        }
    });

    // ---- MAÇ QUEUE İŞLEVSELLİĞİ ----

    // Maç Queue modalını aç
    const openMatchQueueBtn = document.getElementById('openMatchQueueBtn');
    const matchQueueModal = document.getElementById('matchQueueModal');

    if (openMatchQueueBtn) {
        openMatchQueueBtn.addEventListener('click', function () {
            loadMatchQueue();
            matchQueueModal.style.display = 'block';
        });
    }

    // Maç oluşturma formunu gönder
    const createMatchForm = document.getElementById('createMatchForm');

    if (createMatchForm) {
        createMatchForm.addEventListener('submit', function (e) {
            e.preventDefault();

            const formData = new FormData(this);

            // AJAX ile yeni maç oluştur
            fetch('/Admin/CreateMatch', {
                method: 'POST',
                body: formData
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        // Formu temizle
                        createMatchForm.reset();

                        // Queue'yu yeniden yükle
                        loadMatchQueue();

                        alert('Maç başarıyla oluşturuldu ve queue\'ya eklendi.');
                    } else {
                        alert('Hata: ' + data.message);
                    }
                })
                .catch(error => {
                    console.error('Hata:', error);
                    alert('Maç oluşturulurken bir hata oluştu.');
                });
        });
    }

    // Sonraki maç butonunu işle
    const nextMatchBtn = document.getElementById('nextMatchBtn');

    if (nextMatchBtn) {
        nextMatchBtn.addEventListener('click', function () {
            processNextMatch();
        });
    }

    // Maç queue'sunu yükle ve admin panelinde göster
    function loadMatchQueue() {
        fetch('/api/match/queue')
            .then(response => response.json())
            .then(data => {
                const queueContainer = document.getElementById('matchQueueContainer');
                queueContainer.innerHTML = '';

                // Queue boş mu kontrol et
                if (data.queueCount === 0) {
                    queueContainer.innerHTML = '<p>Sırada bekleyen maç bulunmamaktadır.</p>';
                    return;
                }

                // Queue'daki maç sayısını göster
                const queueInfo = document.createElement('div');
                queueInfo.className = 'queue-info';
                queueInfo.textContent = `Sırada ${data.queueCount} maç bulunuyor`;
                queueContainer.appendChild(queueInfo);

                // Maçları listele
                const matchList = document.createElement('ul');
                matchList.className = 'match-queue-list';

                data.matches.forEach((match, index) => {
                    const matchItem = document.createElement('li');
                    matchItem.className = index === 0 ? 'match-item next-match' : 'match-item';
                    matchItem.innerHTML = `
                        <span class="queue-position">${index + 1}</span>
                        <span class="teams">${match.homeTeamName} vs ${match.awayTeamName}</span>
                        <span class="match-date">${new Date(match.matchDate).toLocaleString()}</span>
                        <span class="match-location">${match.location}</span>
                        <span class="match-status">${match.statusText}</span>
                    `;
                    matchList.appendChild(matchItem);
                });

                queueContainer.appendChild(matchList);

                // Bir sonraki maçı göster
                updateNextMatchDisplay(data.matches[0]);
            })
            .catch(error => {
                console.error('Maç queue\'su yüklenirken hata oluştu:', error);
                document.getElementById('matchQueueContainer').innerHTML =
                    '<p>Maç queue\'su yüklenirken bir hata oluştu. Lütfen daha sonra tekrar deneyin.</p>';
            });
    }

    // Bir sonraki maçı işle (dequeue)
    function processNextMatch() {
        if (!confirm('Bir sonraki maçı başlatmak istediğinizden emin misiniz?')) {
            return;
        }

        fetch('/api/match/process-next', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Sırada bekleyen maç bulunamadı.');
                }
                return response.json();
            })
            .then(data => {
                alert(`${data.match.homeTeamName} vs ${data.match.awayTeamName} maçı başlatıldı. Sırada ${data.remainingInQueue} maç kaldı.`);

                // Maç oluşturma ekranını güncelle
                document.getElementById('homeTeamSelect').value = '';
                document.getElementById('awayTeamSelect').value = '';
                document.getElementById('matchDate').value = '';
                document.getElementById('matchLocation').value = '';

                // Queue'yu yeniden yükle
                loadMatchQueue();
            })
            .catch(error => {
                alert('Hata: ' + error.message);
            });
    }

    // Bir sonraki maçı görüntüle
    function updateNextMatchDisplay(match) {
        const nextMatchDisplay = document.getElementById('nextMatchDisplay');
        if (!nextMatchDisplay) return;

        if (!match) {
            nextMatchDisplay.innerHTML = '<p>Sırada bekleyen maç bulunmamaktadır.</p>';
            return;
        }

        nextMatchDisplay.innerHTML = `
            <div class="next-match-info">
                <h3>Sıradaki Maç</h3>
                <div class="match-teams">${match.homeTeamName} vs ${match.awayTeamName}</div>
                <div class="match-details">
                    <span class="match-date">${new Date(match.matchDate).toLocaleString()}</span>
                    <span class="match-location">${match.location}</span>
                </div>
                <div class="match-status">${match.statusText}</div>
            </div>
        `;
    }

    // Takımları seçim kutusuna yükle
    function loadTeamsToSelect() {
        fetch('/Admin/GetTeams')
            .then(response => response.json())
            .then(data => {
                if (!data.success) {
                    console.error('Takımlar yüklenirken hata oluştu:', data.message);
                    return;
                }

                const homeTeamSelect = document.getElementById('homeTeamSelect');
                const awayTeamSelect = document.getElementById('awayTeamSelect');

                if (!homeTeamSelect || !awayTeamSelect) return;

                // Seçim kutularını temizle
                homeTeamSelect.innerHTML = '<option value="">Ev Sahibi Takım Seçin</option>';
                awayTeamSelect.innerHTML = '<option value="">Deplasman Takımı Seçin</option>';

                // Takımları ekle
                data.teams.forEach(team => {
                    const homeOption = document.createElement('option');
                    homeOption.value = team.id;
                    homeOption.textContent = team.name;
                    homeTeamSelect.appendChild(homeOption);

                    const awayOption = document.createElement('option');
                    awayOption.value = team.id;
                    awayOption.textContent = team.name;
                    awayTeamSelect.appendChild(awayOption);
                });
            })
            .catch(error => {
                console.error('Takımlar yüklenirken hata oluştu:', error);
            });
    }

    // Maç oluşturma ekranında tarih seçiciyi ayarla
    const matchDateInput = document.getElementById('matchDate');
    if (matchDateInput) {
        // Minimum tarihi bugün olarak ayarla
        const today = new Date();
        const formattedDate = today.toISOString().split('T')[0];
        matchDateInput.min = formattedDate;

        // Varsayılan değeri bugün olarak ayarla
        matchDateInput.value = formattedDate;
    }

    // Maç oluşturma ekranı açıldığında takımları yükle
    const createMatchBtn = document.getElementById('createMatchBtn');
    if (createMatchBtn) {
        createMatchBtn.addEventListener('click', function () {
            loadTeamsToSelect();

            // Maç oluşturma formunu sıfırla
            document.getElementById('createMatchForm').reset();

            // Tarihi bugün olarak ayarla
            if (matchDateInput) {
                const today = new Date();
                const formattedDate = today.toISOString().split('T')[0];
                matchDateInput.value = formattedDate;
            }

            // Maç oluşturma modalını aç
            document.getElementById('createMatchModal').style.display = 'block';
        });
    }

    // Maç durumunu güncelle
    const updateMatchStatusBtns = document.querySelectorAll('.update-match-status');
    updateMatchStatusBtns.forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.stopPropagation();
            const matchId = this.getAttribute('data-match-id');
            const status = this.getAttribute('data-status');

            fetch('/Admin/UpdateMatchStatus', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    matchId: matchId,
                    status: status
                })
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        // Queue'yu yeniden yükle
                        loadMatchQueue();
                        alert('Maç durumu başarıyla güncellendi.');
                    } else {
                        alert('Hata: ' + data.message);
                    }
                })
                .catch(error => {
                    console.error('Hata:', error);
                    alert('Maç durumu güncellenirken bir hata oluştu.');
                });
        });
    });

    // Maç skorunu güncelle
    const updateScoreForm = document.getElementById('updateScoreForm');
    if (updateScoreForm) {
        updateScoreForm.addEventListener('submit', function (e) {
            e.preventDefault();

            const matchId = document.getElementById('scoreMatchId').value;
            const homeScore = document.getElementById('homeScore').value;
            const awayScore = document.getElementById('awayScore').value;

            fetch('/Admin/UpdateMatchScore', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    matchId: matchId,
                    homeScore: homeScore,
                    awayScore: awayScore
                })
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        // Skoru güncelleme modalını kapat
                        document.getElementById('updateScoreModal').style.display = 'none';

                        // Queue'yu yeniden yükle
                        loadMatchQueue();
                        alert('Maç skoru başarıyla güncellendi.');
                    } else {
                        alert('Hata: ' + data.message);
                    }
                })
                .catch(error => {
                    console.error('Hata:', error);
                    alert('Maç skoru güncellenirken bir hata oluştu.');
                });
        });
    }

    // Skor güncelleme modalını aç
    const updateScoreBtns = document.querySelectorAll('.update-score-btn');
    updateScoreBtns.forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.stopPropagation();
            const matchId = this.getAttribute('data-match-id');
            const homeTeam = this.getAttribute('data-home-team');
            const awayTeam = this.getAttribute('data-away-team');
            const homeScore = this.getAttribute('data-home-score') || '0';
            const awayScore = this.getAttribute('data-away-score') || '0';

            document.getElementById('scoreMatchId').value = matchId;
            document.getElementById('scoreTitle').textContent = `${homeTeam} vs ${awayTeam}`;
            document.getElementById('homeScore').value = homeScore;
            document.getElementById('awayScore').value = awayScore;

            document.getElementById('updateScoreModal').style.display = 'block';
        });
    });

    // Maç silme işlemini onayla
    const deleteMatchBtn = document.getElementById('deleteMatchBtn');
    if (deleteMatchBtn) {
        deleteMatchBtn.addEventListener('click', function () {
            const matchId = document.getElementById('deleteMatchId').value;

            fetch('/Admin/DeleteMatch', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    id: matchId
                })
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        // Silme modalını kapat
                        document.getElementById('deleteMatchModal').style.display = 'none';

                        // Queue'yu yeniden yükle
                        loadMatchQueue();
                        alert('Maç başarıyla silindi.');
                    } else {
                        alert('Hata: ' + data.message);
                    }
                })
                .catch(error => {
                    console.error('Hata:', error);
                    alert('Maç silinirken bir hata oluştu.');
                });
        });
    }

    // Maç silme modalını aç
    const deleteMatchBtns = document.querySelectorAll('.delete-match-btn');
    deleteMatchBtns.forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.stopPropagation();
            const matchId = this.getAttribute('data-match-id');
            const homeTeam = this.getAttribute('data-home-team');
            const awayTeam = this.getAttribute('data-away-team');

            document.getElementById('deleteMatchId').value = matchId;
            document.getElementById('deleteMatchText').textContent = `${homeTeam} vs ${awayTeam} maçını silmek istediğinizden emin misiniz?`;

            document.getElementById('deleteMatchModal').style.display = 'block';
        });
    });

    // Sayfa yüklendiğinde queue'yu yükle
    if (document.getElementById('matchQueueContainer')) {
        loadMatchQueue();
    }

    // Maç oluşturma ekranında takımları yükle
    if (document.getElementById('homeTeamSelect') && document.getElementById('awayTeamSelect')) {
        loadTeamsToSelect();
    }

    // Maç oluşturma ekranında tarih seçiciyi bugün olarak ayarla
    if (matchDateInput) {
        const today = new Date();
        const formattedDate = today.toISOString().split('T')[0];
        matchDateInput.value = formattedDate;
    }
});
