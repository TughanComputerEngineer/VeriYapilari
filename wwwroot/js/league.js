document.addEventListener('DOMContentLoaded', function () {
    // DOM Elements
    const teamSearch = document.getElementById('team-search');
    const teamList = document.getElementById('team-list');
    const noResults = document.getElementById('no-results');
    const filterButtons = document.querySelectorAll('.filter-btn');
    const modal = document.getElementById('team-detail-modal');
    const closeModal = document.querySelector('.close-modal');
    const teamItems = document.querySelectorAll('.team-item');

    // Filter buttons functionality
    filterButtons.forEach(button => {
        button.addEventListener('click', function () {
            // Remove active class from all buttons
            filterButtons.forEach(btn => btn.classList.remove('active'));

            // Add active class to clicked button
            this.classList.add('active');

            const filter = this.getAttribute('data-filter');
            filterTeams(filter);
        });
    });

    // Search functionality
    teamSearch.addEventListener('input', function () {
        const searchTerm = this.value.toLowerCase().trim();
        filterTeamsBySearch(searchTerm);
    });

    // Team item click to show modal
    teamItems.forEach(item => {
        item.addEventListener('click', function (e) {
            // Don't open modal if clicking on favorite button
            if (e.target.closest('.favorite-button') || e.target.closest('form')) {
                return;
            }

            const teamName = this.querySelector('.team-name').textContent.trim();
            const teamRank = this.querySelector('.team-rank').textContent;
            const teamPoints = this.querySelector('.points-circle').textContent;
            const teamMatches = this.querySelector('.team-matches').innerHTML;
            const teamInitial = teamName.charAt(0).toUpperCase();

            // Populate modal
            document.getElementById('modal-team-name').textContent = teamName;
            document.getElementById('modal-team-rank').textContent = teamRank;
            document.getElementById('modal-team-points').textContent = teamPoints;
            document.getElementById('modal-team-matches').innerHTML = teamMatches;
            document.querySelector('.team-detail-logo').textContent = teamInitial;

            // Show modal with animation
            modal.style.display = 'block';
            document.body.style.overflow = 'hidden'; // Prevent scrolling
        });
    });

    // Close modal
    closeModal.addEventListener('click', function () {
        closeModalFunction();
    });

    // Close modal when clicking outside
    window.addEventListener('click', function (e) {
        if (e.target === modal) {
            closeModalFunction();
        }
    });

    // Close modal with Escape key
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape' && modal.style.display === 'block') {
            closeModalFunction();
        }
    });

    // Function to close modal
    function closeModalFunction() {
        modal.style.display = 'none';
        document.body.style.overflow = 'auto'; // Re-enable scrolling
    }

    // Function to filter teams by search term
    function filterTeamsBySearch(searchTerm) {
        let hasVisibleTeams = false;

        teamItems.forEach(item => {
            const teamName = item.getAttribute('data-team-name');

            if (teamName.includes(searchTerm)) {
                item.style.display = '';
                hasVisibleTeams = true;

                // Highlight matching text
                const nameElement = item.querySelector('.team-name');
                const originalName = nameElement.textContent;

                if (searchTerm) {
                    const regex = new RegExp(`(${searchTerm})`, 'gi');
                    const highlightedName = originalName.replace(regex, '<span class="highlight">$1</span>');
                    nameElement.innerHTML = highlightedName;
                } else {
                    nameElement.textContent = originalName;
                }
            } else {
                item.style.display = 'none';
            }
        });

        // Show/hide no results message
        if (hasVisibleTeams) {
            teamList.style.display = '';
            noResults.style.display = 'none';
        } else {
            teamList.style.display = 'none';
            noResults.style.display = 'block';
        }
    }

    // Function to filter teams by category
    function filterTeams(filter) {
        const teams = Array.from(teamItems);

        if (filter === 'all') {
            // Show all teams
            teams.forEach(team => {
                team.style.display = '';
                animateTeamEntry(team, teams.indexOf(team));
            });
        } else if (filter === 'top') {
            // Sort by points and show top 5
            teams.sort((a, b) => {
                const pointsA = parseInt(a.getAttribute('data-team-points'));
                const pointsB = parseInt(b.getAttribute('data-team-points'));
                return pointsB - pointsA;
            });

            teams.forEach((team, index) => {
                if (index < 5) {
                    team.style.display = '';
                    animateTeamEntry(team, index);
                } else {
                    team.style.display = 'none';
                }
            });
        } else if (filter === 'favorites') {
            // Show only favorites
            let hasFavorites = false;

            teams.forEach((team, index) => {
                const isFavorite = team.querySelector('.fa-star.fas');

                if (isFavorite) {
                    team.style.display = '';
                    animateTeamEntry(team, index);
                    hasFavorites = true;
                } else {
                    team.style.display = 'none';
                }
            });

            // Show no results if no favorites
            if (!hasFavorites) {
                teamList.style.display = 'none';
                noResults.style.display = 'block';
                document.querySelector('#no-results .empty-icon').className = 'fas fa-star empty-icon';
                document.querySelector('#no-results p').textContent = 'Henüz favori takımınız bulunmamaktadır.';
            } else {
                teamList.style.display = '';
                noResults.style.display = 'none';
            }
        }
    }

    // Function to animate team entry
    function animateTeamEntry(team, index) {
        team.style.animation = 'none';
        team.offsetHeight; // Trigger reflow
        team.style.animation = `fadeIn 0.3s ease-in-out ${index * 0.05}s forwards`;
    }

    // Add hover animations to match results
    const matchResults = document.querySelectorAll('.match-result');
    matchResults.forEach(result => {
        result.addEventListener('mouseover', function () {
            const resultType = this.textContent.trim();
            let tooltipText = '';

            if (resultType === 'W') tooltipText = 'Galibiyet';
            else if (resultType === 'D') tooltipText = 'Beraberlik';
            else if (resultType === 'L') tooltipText = 'Mağlubiyet';

            // Create and show tooltip
            const tooltip = document.createElement('div');
            tooltip.className = 'tooltip';
            tooltip.textContent = tooltipText;
            document.body.appendChild(tooltip);

            // Position tooltip
            const rect = this.getBoundingClientRect();
            tooltip.style.top = `${rect.top - 30}px`;
            tooltip.style.left = `${rect.left + (rect.width / 2) - (tooltip.offsetWidth / 2)}px`;

            // Remove tooltip on mouseout
            this.addEventListener('mouseout', function () {
                document.body.removeChild(tooltip);
            }, { once: true });
        });
    });

    // Add CSS for tooltip
    const style = document.createElement('style');
    style.textContent = `
        .tooltip {
            position: fixed;
            background-color: rgba(0, 0, 0, 0.8);
            color: white;
            padding: 5px 10px;
            border-radius: 4px;
            font-size: 12px;
            z-index: 1000;
            pointer-events: none;
            animation: fadeIn 0.2s ease;
        }
        
        .tooltip::after {
            content: '';
            position: absolute;
            top: 100%;
            left: 50%;
            margin-left: -5px;
            border-width: 5px;
            border-style: solid;
            border-color: rgba(0, 0, 0, 0.8) transparent transparent transparent;
        }
        
        .highlight {
            background-color: #ffff00;
            color: #000;
            font-weight: bold;
            padding: 0 2px;
            border-radius: 2px;
        }
    `;
    document.head.appendChild(style);

    // Add animations to stats on page load
    const statValues = document.querySelectorAll('.stat-value');
    statValues.forEach((stat, index) => {
        const value = parseInt(stat.textContent);
        stat.textContent = '0';

        // Animate count up
        let current = 0;
        const increment = Math.max(1, Math.floor(value / 30));
        const timer = setInterval(() => {
            current += increment;
            if (current >= value) {
                clearInterval(timer);
                current = value;
            }
            stat.textContent = current;
        }, 30);

        // Add animation to stat item
        const statItem = stat.closest('.stat-item');
        statItem.style.opacity = '0';
        statItem.style.transform = 'translateY(20px)';

        setTimeout(() => {
            statItem.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
            statItem.style.opacity = '1';
            statItem.style.transform = 'translateY(0)';
        }, 100 + (index * 150));
    });

    // Add row hover effect
    teamItems.forEach(item => {
        item.addEventListener('mouseenter', function () {
            const rank = this.querySelector('.team-rank').textContent;

            // Highlight teams with same rank in top 3
            if (rank <= 3) {
                const rankClass = `rank-${rank}`;
                this.classList.add(rankClass);

                // Add trophy icon for top 3
                const rankElement = this.querySelector('.team-rank');
                const originalText = rankElement.textContent;

                if (rank == 1) {
                    rankElement.innerHTML = '<i class="fas fa-trophy" style="color: gold;"></i>';
                } else if (rank == 2) {
                    rankElement.innerHTML = '<i class="fas fa-trophy" style="color: silver;"></i>';
                } else if (rank == 3) {
                    rankElement.innerHTML = '<i class="fas fa-trophy" style="color: #cd7f32;"></i>';
                }

                // Restore original text on mouseleave
                this.addEventListener('mouseleave', function () {
                    rankElement.textContent = originalText;
                    this.classList.remove(rankClass);
                });
            }
        });
    });

    // Add pulse animation to search icon
    const searchIcon = document.querySelector('.search-icon');
    setInterval(() => {
        searchIcon.classList.add('pulse');
        setTimeout(() => {
            searchIcon.classList.remove('pulse');
        }, 1000);
    }, 5000);

    // Add CSS for pulse animation
    const pulseStyle = document.createElement('style');
    pulseStyle.textContent = `
        @keyframes pulse {
            0% { transform: scale(1); }
            50% { transform: scale(1.2); }
            100% { transform: scale(1); }
        }
        
        .pulse {
            animation: pulse 1s ease;
        }
        
        .rank-1 {
            background-color: rgba(255, 215, 0, 0.1);
        }
        
        .rank-2 {
            background-color: rgba(192, 192, 192, 0.1);
        }
        
        .rank-3 {
            background-color: rgba(205, 127, 50, 0.1);
        }
    `;
    document.head.appendChild(pulseStyle);

    // Add sorting functionality
    const headerColumns = document.querySelectorAll('.table-header span');
    headerColumns.forEach(column => {
        column.style.cursor = 'pointer';
        column.title = 'Sıralamak için tıklayın';

        column.addEventListener('click', function () {
            const columnIndex = Array.from(headerColumns).indexOf(this);
            let sortDirection = this.getAttribute('data-sort') === 'asc' ? 'desc' : 'asc';
            this.setAttribute('data-sort', sortDirection);

            // Remove sort indicators from all columns
            headerColumns.forEach(col => {
                col.classList.remove('sorted-asc', 'sorted-desc');
            });

            // Add sort indicator to current column
            this.classList.add(sortDirection === 'asc' ? 'sorted-asc' : 'sorted-desc');

            // Sort teams
            const teams = Array.from(teamItems);
            teams.sort((a, b) => {
                let valueA, valueB;

                // Get values based on column
                if (columnIndex === 0) { // Rank
                    valueA = parseInt(a.querySelector('.team-rank').textContent);
                    valueB = parseInt(b.querySelector('.team-rank').textContent);
                } else if (columnIndex === 1) { // Team name
                    valueA = a.querySelector('.team-name').textContent.trim();
                    valueB = b.querySelector('.team-name').textContent.trim();
                    return sortDirection === 'asc' ? valueA.localeCompare(valueB) : valueB.localeCompare(valueA);
                } else if (columnIndex === 3) { // Points
                    valueA = parseInt(a.querySelector('.points-circle').textContent);
                    valueB = parseInt(b.querySelector('.points-circle').textContent);
                } else {
                    return 0;
                }

                return sortDirection === 'asc' ? valueA - valueB : valueB - valueA;
            });

            // Reorder DOM
            const parent = teamList;
            teams.forEach(team => {
                parent.appendChild(team);
                // Add animation
                team.style.animation = 'none';
                team.offsetHeight; // Trigger reflow
                team.style.animation = 'fadeIn 0.3s ease';
            });
        });
    });

    // Add CSS for sorting indicators
    const sortStyle = document.createElement('style');
    sortStyle.textContent = `
        .sorted-asc::after,
        .sorted-desc::after {
            margin-left: 5px;
            font-family: "Font Awesome 5 Free";
            font-weight: 900;
        }
        
        .sorted-asc::after {
            content: "\\f0d8"; /* up arrow */
        }
        
        .sorted-desc::after {
            content: "\\f0d7"; /* down arrow */
        }
    `;
    document.head.appendChild(sortStyle);

    // Add scroll to top button
    const scrollButton = document.createElement('button');
    scrollButton.className = 'scroll-top-btn';
    scrollButton.innerHTML = '<i class="fas fa-arrow-up"></i>';
    document.body.appendChild(scrollButton);

    // Show/hide scroll button based on scroll position
    window.addEventListener('scroll', function () {
        if (window.pageYOffset > 300) {
            scrollButton.classList.add('show');
        } else {
            scrollButton.classList.remove('show');
        }
    });

    // Scroll to top when button clicked
    scrollButton.addEventListener('click', function () {
        window.scrollTo({
            top: 0,
            behavior: 'smooth'
        });
    });

    // Add CSS for scroll to top button
    const scrollStyle = document.createElement('style');
    scrollStyle.textContent = `
        .scroll-top-btn {
            position: fixed;
            bottom: 20px;
            right: 20px;
            width: 40px;
            height: 40px;
            border-radius: 50%;
            background-color: var(--accent-color);
            color: white;
            border: none;
            cursor: pointer;
            display: flex;
            align-items: center;
            justify-content: center;
            opacity: 0;
            visibility: hidden;
            transition: all 0.3s ease;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);
            z-index: 99;
        }
        
        .scroll-top-btn.show {
            opacity: 1;
            visibility: visible;
        }
        
        .scroll-top-btn:hover {
            background-color: var(--highlight-color);
            transform: translateY(-3px);
        }
    `;
    document.head.appendChild(scrollStyle);
});
