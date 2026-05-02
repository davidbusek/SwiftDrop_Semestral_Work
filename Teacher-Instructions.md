# SwiftDrop - Testing Instructions

## Logins
- **Customer / User:** `123456@sssvt.cz` (Password: `123456`)
- **Restaurant Manager:** `manager@sssvt.cz` (Password: `123456`)
- **Courier:** `courier@sssvt.cz` (Password: `123456`)
- **Admin:** `admin@sssvt.cz` (Password: `123456`)

---

## Testing Workflow

### Step 1: Place an Order (Customer)
1. In your browser, navigate to the application and **Login** with the User account (`123456@sssvt.cz`).
2. Go to **Restaurants** from the navigation bar.
3. Select any restaurant and add some items to your cart. *(Since this is a multi-restaurant system, you can also go back and add items from a different restaurant if you want to test the full potential of SwiftDrop).*
4. Navigate to your **Cart**.
5. Click on **Payment**.
6. **Log out** once the order is successfully placed.

### Step 2: Prepare the Order (Manager)
1. **Login** with the Manager account (`manager@sssvt.cz`).
2. Go to the **Restaurant Dashboard**. 
3. You should see the new order under the **"Incoming Requisitions"**.
4. Click the **"Start Prep"** button (this signifies the restaurant has started cooking/preparing the items).
5. Once preparation is simulated as done, click the **"Ready for Courier"** button.
6. **Log out**.

### Step 3: Deliver the Order (Courier)
1. **Login** with the Courier account (`courier@sssvt.cz`).
2. Go to the **Active Deliveries**.
3. You will see the newly prepared order available for delivery (in the bottom).
4. You can click the **"Show Route"** button to view the pickup and drop-off locations.
5. Click **"Accept Job"** to start the delivery process.
6. Once you've virtually arrived at the customer's location, click **"Complete"** to mark the delivery as finished.

---

