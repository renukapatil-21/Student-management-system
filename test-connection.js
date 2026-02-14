const { MongoClient } = require('mongodb');

const uri = "mongodb+srv://patilrenuka43_db_user:StudentManagementDB@studentmanagementdb.2qkviwx.mongodb.net/StudentManagementDB?retryWrites=true&w=majority";

async function testConnection() {
  const client = new MongoClient(uri, {
    serverSelectionTimeoutMS: 5000,
    connectTimeoutMS: 10000,
  });

  try {
    console.log('Attempting to connect to MongoDB Atlas...');
    await client.connect();
    console.log('✅ Successfully connected to MongoDB Atlas!');
    
    // Test a simple operation
    const db = client.db('StudentManagementDB');
    const collections = await db.listCollections().toArray();
    console.log('Collections:', collections.map(c => c.name));
    
  } catch (error) {
    console.error('❌ Failed to connect to MongoDB Atlas:');
    console.error(error.message);
  } finally {
    await client.close();
  }
}

testConnection();