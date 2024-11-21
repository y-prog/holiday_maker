# GitHub(Hygien)

P U S H

1. Git Push sker endast ihop med gruppen, det vill säga ingen Push görs på egen hand utan att gruppmedlemmarna närvarar.

2. Vid Push sätter användaren igång sin skärm delning detta p.g.a. att säkerställa att gruppen förstår koden som ska Pushas och om den är nödvändig för huvud koden.

3. När Push är godkänd och genomförd ser resterande av gruppen att göra en Pull på den nya ändringen,  för att säkerställa detta går vi laget runt och checkar av om man gjort det.

4. Push sker vid två tillfällen under dagen. Dessa tider är 11.30 & 15.30 , det är då viktigt att grupp medlemmarna är tillgängliga vid denna tid.

M E R G E

1. Tänk på  att kontinuerligt hämta uppdaterad huvud kod till er branch. Detta för att motverka att ni jobbar på en gammal huvud kod!

2. Genom att skriva git merge main inne i er branch hämtar ni den uppdaterade versionen av huvud koden till en branch.
Tips : Dubbelkolla vilken branch ni är inne i innan ni kör en merge. Detta för att undvika att ni kör merge till fel branch!

3. När ni är redo att ladda upp er nya kod till huvud koden efter flertalet tester inne i er egen branch byter ni branch till main detta genom att skriva git checkout main.
Väl inne i main skriver ni följande git merge <namn på er branch> för att hämta er kod in till main.

B R A N C H

1. Skapa Branch efter feature namn som koden innehåller och inte efter ert eget namn. Ett exempel kan vara en branch som ska innehålla kod för checka tillgänligheten på rum. Den kan då heta feature/checkAvailability

2. I er egen branch kan nu köra add, commit och push utan konstigheter! Detta kan vara bra för att resterande grupp kan via GitHub få tillgänglighet till er kod som ni jobbar på i er branch om ni skulle behöva  hjälp.
