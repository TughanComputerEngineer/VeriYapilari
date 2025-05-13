document.addEventListener("DOMContentLoaded", () => {
    console.log("✅ DOMContentLoaded fired — home.js yüklendi ve çalışıyor.");

    const searchInput = document.getElementById("team-search");
    const teamItems = document.querySelectorAll(".team-item");
    const noResults = document.getElementById("no-results");
    const teamContainer = document.querySelector(".teams-container");

    class TeamNode {
        constructor(id, name, league, score, last5Match) {
            this.teamId = id;
            this.teamName = name;
            this.currentLeague = league;
            this.teamScore = score;
            this.last5Match = last5Match;
            this.left = null;
            this.right = null;
        }
    }

    class TeamTree {
        constructor() {
            this.root = null;
        }
        insert(id, name, league, score, last5Match) {
            const newNode = new TeamNode(id, name, league, score, last5Match);
            if (this.root === null) {
                this.root = newNode;
            } else {
                this._insertRec(this.root, newNode);
            }
        }
        _insertRec(current, newNode) {
            if (newNode.teamId < current.teamId) {
                if (current.left === null) current.left = newNode;
                else this._insertRec(current.left, newNode);
            } else {
                if (current.right === null) current.right = newNode;
                else this._insertRec(current.right, newNode);
            }
        }
        inOrderTraversal() {
            const result = [];
            this._inOrderRec(this.root, result);
            return result;
        }
        _inOrderRec(node, result) {
            if (!node) return;
            this._inOrderRec(node.left, result);
            result.push(node);
            this._inOrderRec(node.right, result);
        }
        printTree() {
            if (!this.root) {
                console.warn("⚠️ Ağaç boş!");
                return;
            }
            console.log("🌳 Binary Search Tree yapısı:");
            this._prettyPrint(this.root);
        }
        _prettyPrint(node, prefix = "", isLeft = true) {
            if (!node) return;
            if (node.right) {
                this._prettyPrint(node.right,
                    `${prefix}${isLeft ? "│   " : "    "}`,
                    false
                );
            }
            console.log(
                `${prefix}${isLeft ? "└── " : "┌── "}${node.teamName} (ID: ${node.teamId})`
            );
            if (node.left) {
                this._prettyPrint(node.left,
                    `${prefix}${isLeft ? "    " : "│   "}`,
                    true
                );
            }
        }
        searchByName(q) {
            const res = [];
            this._searchByNameRec(this.root, q.toLowerCase().trim(), res);
            return res;
        }
        _searchByNameRec(node, q, res) {
            if (!node) return;
            this._searchByNameRec(node.left, q, res);
            if (node.teamName.toLowerCase().includes(q)) res.push(node);
            this._searchByNameRec(node.right, q, res);
        }
    }

    // MaxHeap veri yapısı - Takımları puana göre sıralamak için
    class MaxHeap {
        constructor() {
            this.heap = [];
        }

        // Heap'e yeni takım ekleme
        insert(team) {
            this.heap.push(team);
            this.heapifyUp(this.heap.length - 1);
        }

        // Heap'i yukarı doğru düzenleme
        heapifyUp(index) {
            const parent = Math.floor((index - 1) / 2);

            if (index > 0 && this.heap[parent].teamScore < this.heap[index].teamScore) {
                // Eğer ebeveyn düğümün puanı daha düşükse, yer değiştir
                [this.heap[parent], this.heap[index]] = [this.heap[index], this.heap[parent]];
                this.heapifyUp(parent);
            }
        }

        // En yüksek puanlı takımı çıkar
        extractMax() {
            if (this.heap.length === 0) return null;

            const max = this.heap[0];
            const last = this.heap.pop();

            if (this.heap.length > 0) {
                this.heap[0] = last;
                this.heapifyDown(0);
            }

            return max;
        }

        // Heap'i aşağı doğru düzenleme
        heapifyDown(index) {
            const left = 2 * index + 1;
            const right = 2 * index + 2;
            let largest = index;

            if (left < this.heap.length && this.heap[left].teamScore > this.heap[largest].teamScore) {
                largest = left;
            }

            if (right < this.heap.length && this.heap[right].teamScore > this.heap[largest].teamScore) {
                largest = right;
            }

            if (largest !== index) {
                [this.heap[index], this.heap[largest]] = [this.heap[largest], this.heap[index]];
                this.heapifyDown(largest);
            }
        }

        // Heap'in boyutu
        size() {
            return this.heap.length;
        }

        // Heap'i yazdır (debug için)
        printHeap() {
            console.log("🔍 MaxHeap yapısı (Puana göre sıralı):");
            this.heap.forEach((team, index) => {
                console.log(`${index}: ${team.teamName} - ${team.teamScore} puan`);
            });
        }

        // İsme göre arama
        searchByName(query) {
            query = query.toLowerCase().trim();
            return this.heap.filter(team =>
                team.teamName.toLowerCase().includes(query)
            );
        }
    }

    const teamTree = new TeamTree();
    const teamHeap = new MaxHeap(); // Yeni MaxHeap oluştur

    // Takımları BST'ye ekle
    teamItems.forEach(item => {
        // data-id yerine data-team-name kullanıldığı için rank değerini alıyoruz
        const id = parseInt(item.querySelector(".team-rank")?.textContent || "0", 10);
        const name = item.querySelector(".team-name")?.textContent || "";
        const league = item.querySelector(".team-league")?.textContent || "";
        const score = parseInt(item.querySelector(".team-points")?.textContent || "0", 10);
        const last5 = ""; // Bu değer mevcut değilse boş string kullan

        teamTree.insert(id, name, league, score, last5);

        // Aynı takımı MaxHeap'e de ekle
        const teamNode = new TeamNode(id, name, league, score, last5);
        teamHeap.insert(teamNode);
    });

    console.log("🔍 Tree root ve toplam düğüm sayısı:",
        teamTree.root,
        teamTree.inOrderTraversal().length
    );

    teamTree.printTree();
    console.log("\n📋 Takımlar (ID'ye göre sıralı):");
    teamTree.inOrderTraversal().forEach(t =>
        console.log(`ID: ${t.teamId}, Takım: ${t.teamName}`)
    );

    // MaxHeap yapısını yazdır
    teamHeap.printHeap();

    // Takımları puana göre sıralayarak göster
    function renderTeamsByScore() {
        // Önce mevcut takımları temizle
        const teamContainer = document.querySelector(".teams-container");
        if (!teamContainer) return;

        teamContainer.innerHTML = '';

        // Geçici bir heap oluştur (orijinal heap'i korumak için)
        const tempHeap = new MaxHeap();
        teamHeap.heap.forEach(team => tempHeap.insert({ ...team }));

        // En yüksek puandan en düşüğe doğru takımları çıkar ve göster
        let rank = 1;
        while (tempHeap.size() > 0) {
            const team = tempHeap.extractMax();

            // Takım HTML'ini oluştur
            const teamElement = document.createElement('div');
            teamElement.className = 'team-item';
            teamElement.innerHTML = `
                <span class="team-rank">${rank}</span>
                <span class="team-name">${team.teamName}</span>
                <span class="team-league">${team.currentLeague}</span>
                <span class="team-points">${team.teamScore}</span>
            `;

            // Animasyon efekti
            teamElement.style.opacity = 0;
            teamElement.style.transform = "translateY(10px)";

            // DOM'a ekle
            teamContainer.appendChild(teamElement);

            // Animasyonu başlat
            setTimeout(() => {
                teamElement.style.transition = "opacity 0.3s, transform 0.3s";
                teamElement.style.opacity = 1;
                teamElement.style.transform = "translateY(0)";
            }, rank * 50);

            rank++;
        }

        // "Sonuç bulunamadı" mesajını ekle
        const noResultsElement = document.createElement('div');
        noResultsElement.id = 'no-results';
        noResultsElement.style.display = 'none';
        noResultsElement.textContent = 'Aradığınız takım bulunamadı.';
        teamContainer.appendChild(noResultsElement);

        // Arama inputunu güncellenmiş DOM'a göre ayarla
        const searchInput = document.getElementById("team-search");
        if (searchInput) {
            const teamItems = document.querySelectorAll(".team-item");
            const noResults = document.getElementById("no-results");

            searchInput.addEventListener("input", () => {
                const q = searchInput.value.toLowerCase().trim();
                if (!q) {
                    teamItems.forEach(i => (i.style.display = "grid"));
                    noResults.style.display = "none";
                    return;
                }

                const results = teamTree.searchByName(q);
                console.log(`"${q}" arama sonuçları:`, results.map(r => r.teamName));

                let found = false;
                teamItems.forEach(item => {
                    // Takım adını doğrudan karşılaştır
                    const teamName = item.querySelector(".team-name")?.textContent || "";
                    const match = teamName.toLowerCase().includes(q);

                    item.style.display = match ? "grid" : "none";
                    if (match) found = true;
                });

                noResults.style.display = found ? "none" : "block";
            });
        }
    }

    // Sayfa yüklendiğinde MaxHeap ile sıralanmış takımları göster
    renderTeamsByScore();

    // Mevcut arama fonksiyonu
    searchInput.addEventListener("input", () => {
        const q = searchInput.value.toLowerCase().trim();
        if (!q) {
            teamItems.forEach(i => (i.style.display = "grid"));
            noResults.style.display = "none";
            return;
        }

        const results = teamTree.searchByName(q);
        console.log(`"${q}" arama sonuçları:`, results.map(r => r.teamName));

        let found = false;
        teamItems.forEach(item => {
            // Takım adını doğrudan karşılaştır
            const teamName = item.querySelector(".team-name")?.textContent || "";
            const match = teamName.toLowerCase().includes(q);

            item.style.display = match ? "grid" : "none";
            if (match) found = true;
        });

        noResults.style.display = found ? "none" : "block";
    });

    // Animasyon efekti (orijinal koddan)
    teamItems.forEach((item, idx) => {
        item.style.opacity = 0;
        item.style.transform = "translateY(10px)";
        setTimeout(() => {
            item.style.transition = "opacity 0.3s, transform 0.3s";
            item.style.opacity = 1;
            item.style.transform = "translateY(0)";
        }, idx * 50);
    });

    // En yüksek puanlı takımı konsola yazdır
    console.log("🏆 En yüksek puanlı takım:", teamHeap.heap[0]?.teamName, "- Puan:", teamHeap.heap[0]?.teamScore);
});
