// Import the functions you need from the SDKs you need
import { initializeApp } from "firebase/app";
import { getAnalytics } from "firebase/analytics";
import { getAuth } from "firebase/auth";
import { getFirestore } from "firebase/firestore";

// Your web app's Firebase configuration
// For Firebase JS SDK v7.20.0 and later, measurementId is optional
const firebaseConfig = {
  apiKey: "AIzaSyAXV3b32y6FczuPglBSPK1vYMwUBTR2PJY",
  authDomain: "futureinnovators-c9f9d.firebaseapp.com",
  projectId: "futureinnovators-c9f9d",
  storageBucket: "futureinnovators-c9f9d.firebasestorage.app",
  messagingSenderId: "96255202479",
  appId: "1:96255202479:web:8a1e95e9d7c0706636f05a",
  measurementId: "G-3LNPXDZMG8"
};

// Initialize Firebase
const app = initializeApp(firebaseConfig);
const analytics = getAnalytics(app);
const auth = getAuth(app);
const db = getFirestore(app);

export { auth, db, app };