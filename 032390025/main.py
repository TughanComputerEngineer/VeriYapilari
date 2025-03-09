import pygame

pygame.init()

#TODO daha iyi bir font bul. UI ı geliştir.
FONT = pygame.font.Font("MinimalPixelFont.ttf", 64)
screen = pygame.display.set_mode((600, 600))
pygame.display.set_caption("League")


#TODO butonu optimize et
class Button():
    def __init__(self, text, x, y, width, height, color):
        self.screen = pygame.display.get_surface()

        self.rect = pygame.Rect(x, y, width, height)
        self.color = color
        self.copy = text

        self.clicked = False

        self.text = FONT.render(self.copy, True, "black")
        self.text_rect = self.text.get_rect()

        self.text_rect.center = (x + width/2, y + height/3)

    def interaction(self):
        mouse_pos = pygame.mouse.get_pos()
        if self.text_rect.collidepoint(mouse_pos) and pygame.mouse.get_pressed()[0]:
            self.clicked = True

    def draw(self):
        pygame.draw.rect(self.screen, self.color, self.rect)
        self.screen.blit(self.text, self.text_rect)

class Team():
    def __init__(self, name, win, lose, draw):
        self.name = name
        self.win = win
        self.lose = lose
        self.draw = draw

        self.queue = len(teams) # değiştirilecek
        self.score = 0

    def calculate_score(self):
        self.score += self.win * 3 + self.draw

def get_name(text1 = ""):
    name = ""
    running = True
    while running:
        screen.fill("white")
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                running = False
            if event.type == pygame.KEYDOWN:
                if event.key == pygame.K_RETURN:
                    running = False
                else:
                    if (pygame.key.name(event.key) == "space"):
                        name += " "
                    elif (pygame.key.name(event.key) == "backspace"):
                        name = name[:-1]
                    else:
                        name += pygame.key.name(event.key)

        menu_text = FONT.render(text1, True, "black")
        menu_rect = menu_text.get_rect()
        menu_rect.center = (screen.get_width()/2, 50)

        text = FONT.render(name, True, "black")
        rect = text.get_rect()
        rect.center = (screen.get_width() / 2, screen.get_height() / 2)

        screen.blit(menu_text, menu_rect)
        screen.blit(text, rect)

        pygame.display.update()
    return name.strip()

def create_team():
    name = get_name()
    teams.append(Team(name, 0, 0, 0))

def delete_team():
    name = get_name()
    for team in teams:
        if(team.name == name):
            teams.remove(team)

def start_league():
    team1 = get_name("team 1 name")
    team2 = get_name("team 2 name")

    team1_button = Button(team1 + " WON!", 40, 20, 100, 50, "white")
    team2_button = Button(team2 + " WON!", 40, 100, 100, 50, "white")
    draw_button = Button("DRAW!", 40, 200, 100, 50, "white")

    Buttons = {"team1": team1_button,
               "team2": team2_button,
               "draw": draw_button}

    if not any(team.name == team1 for team in teams) or not any(team.name == team2 for team in teams) or len(teams) < 2:
        print("ERROR")
        return

    else:
        running = True
        while running:
            screen.fill("white")
            for event in pygame.event.get():
                if event.type == pygame.QUIT:
                    running = False
                if event.type == pygame.KEYDOWN:
                    if event.key == pygame.K_RETURN:
                        running = False

            for button_name, button in zip(Buttons, Buttons.values()):
                button.draw()
                button.interaction()

                if button.clicked:
                    if button_name == "team1":
                        for team in teams:
                            if team.name == team1:
                                team.score+=1
                    elif button_name == "team2":
                        for team in teams:
                            if team.name == team2:
                                team.score += 1
                    elif button_name == "draw":
                        for team in teams:
                            if team.name == team1 or team.name == team2:
                                team.score += 1
                    button.clicked = False
                    running = False
                    break
            pygame.display.update()


buttons = {"create team": Button("create team", 400, 100, 100, 50, "white"),
            "delete team": Button("delete team", 400, 200, 100, 50, "white"),
            "start league": Button("start league", 400, 300, 100, 50, "white")}

# TODO teams i heap tree olarak değiştir!
teams = []

def main_menu():
    running = True
    while running:
        screen.fill("white")

        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                running = False
            if event.type == pygame.KEYDOWN:
                if event.key == pygame.K_ESCAPE:
                    running = False

        for team in teams:
            text = FONT.render(team.name, True, "black")
            rect = text.get_rect()

            rect.topleft = (10, 32 * (team.queue))
            screen.blit(text, rect)

        for button_name, button in zip(buttons, buttons.values()):
            button.draw()
            button.interaction()

            if button.clicked:
                if button_name == "create team":
                    create_team()
                elif button_name == "delete team":
                    delete_team()
                elif button_name == "start league":
                    start_league()
                button.clicked = False

        pygame.display.update()
    pygame.quit()

main_menu()

for team in teams:
    print(team.name + " ", team.score)