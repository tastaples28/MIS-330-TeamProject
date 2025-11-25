# JawsDB Database Setup Guide

## Connection Information

Your Heroku JawsDB connection details:
- **Host**: `y5s2h87f6ur56vae.cbetxkdyhwsb.us-east-1.rds.amazonaws.com`
- **Username**: `v3361rseg8fhl3du`
- **Password**: `gq0lwxcy7b0bk035`
- **Port**: `3306`
- **Database**: `bb9p6wqs44wmf7vj`
- **Connection Name** (in MySQL Workbench): `mis330-tp`

## Setup Instructions

### Option 1: Using MySQL Workbench (Recommended)

1. **Open MySQL Workbench**
   - You've already added the connection as "mis330-tp"
   - Double-click on "mis330-tp" to connect

2. **Open the Schema File**
   - Click **File → Open SQL Script**
   - Navigate to: `Database/schema_jawsdb.sql`
   - Click "Open"

3. **Execute the Script**
   - Make sure you're connected to the database
   - Click the **Execute** button (⚡ lightning bolt icon) or press `Ctrl + Shift + Enter`
   - Wait for "Script executed successfully" message

4. **Verify Tables Created**
   - In the left sidebar (SCHEMAS panel), expand your database
   - Expand "Tables"
   - You should see these tables:
     - `customer`
     - `employee`
     - `furniture`
     - `CustomerPurchaseTransaction`
     - `TransactionDetail`

### Option 2: Using Command Line

1. **Connect to JawsDB**:
```bash
mysql -h y5s2h87f6ur56vae.cbetxkdyhwsb.us-east-1.rds.amazonaws.com -P 3306 -u v3361rseg8fhl3du -p'gq0lwxcy7b0bk035' bb9p6wqs44wmf7vj
```

2. **Run the schema**:
```bash
mysql -h y5s2h87f6ur56vae.cbetxkdyhwsb.us-east-1.rds.amazonaws.com -P 3306 -u v3361rseg8fhl3du -p'gq0lwxcy7b0bk035' bb9p6wqs44wmf7vj < Database/schema_jawsdb.sql
```

### Option 3: Using HeidiSQL (as mentioned in JawsDB console)

1. **Open HeidiSQL**
2. **Create New Session**:
   - Host: `y5s2h87f6ur56vae.cbetxkdyhwsb.us-east-1.rds.amazonaws.com`
   - User: `v3361rseg8fhl3du`
   - Password: `gq0lwxcy7b0bk035`
   - Port: `3306`
   - Database: `bb9p6wqs44wmf7vj`
3. **Connect**
4. **Open SQL File**: File → Load SQL file → Select `Database/schema_jawsdb.sql`
5. **Execute**

## Verify Setup

Run this query to verify all tables were created:

```sql
SHOW TABLES;
```

You should see:
- customer
- employee
- furniture
- CustomerPurchaseTransaction
- TransactionDetail

## Connection String Format

For use in your C# backend (.NET), you can use this connection string:

```
Server=y5s2h87f6ur56vae.cbetxkdyhwsb.us-east-1.rds.amazonaws.com;Port=3306;Database=bb9p6wqs44wmf7vj;Uid=v3361rseg8fhl3du;Pwd=gq0lwxcy7b0bk035;
```

Or using the full connection URL format:
```
mysql://v3361rseg8fhl3du:gq0lwxcy7b0bk035@y5s2h87f6ur56vae.cbetxkdyhwsb.us-east-1.rds.amazonaws.com:3306/bb9p6wqs44wmf7vj
```

## Troubleshooting

### "Access denied"
- Double-check the username and password
- Make sure you're using the correct host and port

### "Can't connect to MySQL server"
- Verify your internet connection
- Check if the JawsDB instance is active (free tier may sleep after inactivity)

### "Table already exists"
- The schema file includes `DROP TABLE IF EXISTS` statements, so this should be handled automatically
- If you get errors, you can manually drop tables and re-run the script

