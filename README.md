# 🛒 Auction Web System – Final Year Project

A full-featured online auction platform built with **ASP.NET Core MVC**. The system supports secure registration, auction creation, real-time bidding, and complete admin control — simulating a professional auction environment with enforced rules and validations.

---

## 📸 Project Screenshots

> Replace the image links below (`assets/your_image.png`) with your actual screenshots.

| Feature | Screenshot |
|--------|------------|
| Login Page | ![Image](https://github.com/user-attachments/assets/b3cde781-0707-498f-a6e9-23c7c7df170e) |
| Registration Page | ![Image](https://github.com/user-attachments/assets/01a465a3-ec20-4b09-ba30-55b453a18ef1) |
| Browse Auctions | ![Image](https://github.com/user-attachments/assets/8aa3bd26-ea80-4807-8984-747092f7b970) |
| Auction Details | ![Image](https://github.com/user-attachments/assets/21ca45e9-56ac-4e71-bec2-3a4d564acf93) |
| My Bids | ![Image](https://github.com/user-attachments/assets/97fbe799-84cc-4582-b7c5-f838a1696804) |
| Bidders | ![Image](https://github.com/user-attachments/assets/a7604331-ee24-44b0-ad25-da9ac0060e7e) |
| My Auctions | ![Image](https://github.com/user-attachments/assets/83bd8d67-c722-4872-936e-448a2eb252e4) |
| Won Auctions | ![Image](https://github.com/user-attachments/assets/dc271f61-791e-4003-9d00-051bc28aafea) |
| Admin Dashboard | ![Image](https://github.com/user-attachments/assets/4db5d125-a9a4-4d0c-9f9c-1b95e694ae28) |
| Admin Pending Auctions | ![Image](https://github.com/user-attachments/assets/38b0b422-7862-445d-b4ce-6cc647bb3191) |
| Admin Sell Approval | ![Image](https://github.com/user-attachments/assets/2a6cd409-e64b-477a-a1b1-5d920dfe09e4) |
| All Users | ![Image](https://github.com/user-attachments/assets/e1e03df1-8b1f-4142-83b4-753d3d426a33) |

---

## 🔐 Authentication & Identity

- Role-based system using **ASP.NET Core Identity**
- Two roles: `User` and `Admin`
- Secure login and registration forms with validation
- **Blocked users** cannot log in until unblocked by an admin

---

## 🧭 User Features

### ✅ Register & Login
- Two factory methods for account creation (user & admin)
- Strong password enforcement
- Phone number entry during registration
- Role-based redirection (Admins → Admin Dashboard, Users → Auction Home)

### 🏷️ Browse Auctions
- Lists all **approved & unsold** auctions
- Search by auction name
- Displayed in a modern card layout with badges and timestamps

### 🔍 Auction Details
- Full item info: title, price, end time, owner email
- Uploaded images shown (max 3)
- Users can place bids on active auctions

### 🧺 My Auctions
- Shows all user's auctions with status: `Pending`, `Approved`, `Rejected`, `Sold`
- Auctions can be edited or deleted **only before approval**
- Full bid list and image management per auction

### 🏆 Won Auctions
- Displays auctions the user has won
- Owner can click **Sell to Bidder** for a manual sale
- If no action is taken, system auto-sells to the highest bidder and requests admin approval

---

## 🛒 Selling to Bidder

- Owners can sell ended auctions to the top bidder
- Admin must approve before status changes to `Sold`
- If owner does not sell manually, system **auto-selects top bidder**
- Ensures accountability and delivery validation

---

## 🧑‍💼 Admin Features

### 🔎 Auction Management
- View `Pending` auctions
- Approve, Reject, or Cancel auctions
- Full access to auction and user details

### 🛠️ User Management
- Block or Unblock users
- Blocked users lose access to login and system features

### 📜 Admin Logs
- Logs all admin actions:
  - Action type (approve, reject, block, etc.)
  - Affected auction or user
  - Timestamp
- Enables system transparency and traceability

### ✅ Approve Sales
- Admin verifies and approves **final sale**
- Marks auction as officially `Sold`

---

## 💸 Bidding System

- Users can place bids on approved, active auctions
- Bids must be **higher** than both the starting price and current highest
- Bids stored with amount, timestamp, and bidder identity
- Once auction ends:
  - Further bids are blocked
  - Top bidder becomes winner
  - Auction appears in their **Won Auctions**

---

## 🔐 System Rules & Validations

The application enforces critical rules for fairness, security, and data integrity:

- ✅ Two **factory methods** for registration: user and admin
- 🚫 **Blocked users** cannot access the system
- 🔐 Strong **authorization**: users cannot access admin routes or manipulate URLs
- 🔑 **Password strength**: weak passwords are rejected
- 📞 **Phone number** is required at registration
- 🖼️ Max **3 uploaded images**, each ≤ 3MB
- 🗓️ Auction **end date must be in the future**
- 💵 **Bids must be higher** than both the original and current highest price
- ⏳ If owner doesn't sell manually after auction ends, the system:
  - Auto-sells to top bidder
  - Sends for **admin approval**

---

## 🧱 Tech Stack

- **ASP.NET Core MVC**
- **Entity Framework Core**
- **SQL Server**
- **ASP.NET Identity**
- **Bootstrap 5 / Razor Views**

---

