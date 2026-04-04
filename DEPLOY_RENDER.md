# Deploy to Render.com (Free)

## Step 1: Push code to GitHub
1. Create a GitHub repo (https://github.com/new)
2. In your project folder, run:
   ```bash
   git init
   git add .
   git commit -m "Initial commit: Sleep Factors Blazor app with auth"
   git branch -M main
   git remote add origin https://github.com/YOUR_USERNAME/sleep-factors-app.git
   git push -u origin main
   ```

## Step 2: Set up Render
1. Go to https://render.com (sign up with Google/GitHub)
2. Click **New +** → **Web Service**
3. Connect your GitHub repo
4. Fill in:
   - **Name:** sleep-factors-app
   - **Runtime:** Docker
   - **Build Command:** (leave empty, Dockerfile will handle it)
   - **Start Command:** (leave empty, Dockerfile will handle it)
   - **Instance Type:** Free (sleeps after 15 min inactivity)

5. Click **Create Web Service** and wait ~5 min for first deploy

## Step 3: Access your app
- Your app will be live at: `https://sleep-factors-app.onrender.com`
- First load after sleep may take ~3 seconds to wake up

## Step 4: First-time setup
1. Visit the app and click **Register**
2. Create your account (e.g., email: yourname@gmail.com, password: ...)
3. Start tracking sleep factors!

## Notes
- SQLite database persists on the container (data survives redeploys)
- Free tier sleeps after 15 min of inactivity (automatic wake on next request)
- No card required unless you want a paid plan
- Render auto-redeploys when you push to GitHub

## Updating the app
1. Make changes locally and commit: `git commit -am "Your changes"`
2. Push to GitHub: `git push`
3. Render auto-redeploys within 2-3 min
