document.addEventListener("DOMContentLoaded", () => {
    console.log("✅ DOMContentLoaded fired — home.js yüklendi ve çalışıyor.");

    const searchInput = document.getElementById("team-search");
    const teamItems = document.querySelectorAll(".team-item");
    const noResults = document.getElementById("no-results");

    class TeamNode {
        constructor(id, name, score, last5Match) {
            this.teamId = id;
            this.teamName = name;
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
        insert(id, name, score, last5Match) {
            const newNode = new TeamNode(id, name, score, last5Match);
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

    const teamTree = new TeamTree();

    teamItems.forEach(item => {
        const id = parseInt(item.dataset.id || "0", 10);
        const name = item.querySelector(".team-name")?.textContent || "";
        const score = parseInt(item.querySelector(".team-score")?.textContent || "0", 10);
        const last5 = item.querySelector(".last-matches")?.dataset.matches || "";
        teamTree.insert(id, name, score, last5);
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

    searchInput.addEventListener("input", () => {
        const q = searchInput.value;
        if (!q) {
            teamItems.forEach(i => (i.style.display = "grid"));
            noResults.style.display = "none";
            return;
        }
        const results = teamTree.searchByName(q);
        console.log(`"${q}" arama sonuçları:`, results.map(r => r.teamName));

        let found = false;
        teamItems.forEach(item => {
            const id = parseInt(item.dataset.id || "0", 10);
            const match = results.some(r => r.teamId === id);
            item.style.display = match ? "grid" : "none";
            if (match) found = true;
        });
        noResults.style.display = found ? "none" : "block";
    });

    teamItems.forEach((item, idx) => {
        item.style.opacity = 0;
        item.style.transform = "translateY(10px)";
        setTimeout(() => {
            item.style.transition = "opacity 0.3s, transform 0.3s";
            item.style.opacity = 1;
            item.style.transform = "translateY(0)";
        }, idx * 50);
    });
});
