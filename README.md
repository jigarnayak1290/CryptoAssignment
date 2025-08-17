### Merkle tree
#### 1. Developing a Merkle tree with limited functionality includes:
1.1 Constructing the Merkle tree using leaf nodes derived from [User ID] and [User Balance] pairs.
 - Each leaf node is typically the hash of the concatenated or structured representation of User ID and User Balance.
 - Intermediate nodes are computed by hashing pairs of child nodes recursively up to the root.

1.2 Fetching the Merkle path for a specific user:
 - This path consists of the sibling hashes required to verify the inclusion of the user's leaf node in the Merkle root.
 - The path does not include the Merkle root itself, only the hashes needed to recompute it from the leaf.


#### 2. Sequence Diagrams

##### 2.1 Program Start-up
```plaintext
Program.cs			AppDbContext     UserDataInitializer     MerkleTreeLibrary     UserMerkleTreeRepo
	|					|                   |                   	|                       |
Initialize InMeomryDB   |                   |                   	|                       |
(With Sample User)      |                   |                   	|                       |
	|-----------------> |                   |                   	|                       |
	|		Return DB with Sample User		|                   	|                       |
	| <-----------------|                   |                   	|                       |
Send DB users   		|                   |                   	|                       |
	|-------------------------------------> |                   	|                       |
	|               	|	   Call Merkle Library with user info	|                       |
	|               	|                   |---------------------> |                       |
	|               	|					|			  Crete Merkle Tree               	|
	|               	|					|			(From Users & Balance)              |
	|               	|					| <---------------------|						|
	|               	|	      Send User Merkle Tree to Repo		|                       |
	|               	|					|---------------------------------------------> |
	|					|                   |                   	|			Save User Merkle Tree
	|               	|					|						|				(For reference)
Start WebApi <------------------------------| <---------------------------------------------|
```
##### 2.2 Get Merkle root of [User Id] & [User Balance]
```plaintext
Client     	WebApi-Get     	UserEnquiryController	UserEnquiryService	UserMerkleTreeRepo
	|			|					|						|					|
	|---------> |					|						|					|
	|	getmerklerootofusers		|						|					|
	|			|-----------------> |                       |                   |
	|			|		  Call UserEnquiry Service       	|                   |
	|			|					|---------------------> |                   |
	|			|					|			Fetch Merkle Tree from Repo     |
	|			|					|						|-----------------> |
	|			|					|						|			 Return Merkle Tree
Merkle Tree	<---| <-----------------| <---------------------| <-----------------|
```
##### 2.3 Get Merkle path (proof) of specific [User Id] and return it with User balance.
```plaintext
Client     	WebApi-Get     	UserEnquiryController	UserEnquiryService	InMemoryDB	UserMerkleTreeRepo	MerkleTreeLibrary
	|			|					|						|				|				|					|
	|---------> |					|						|				|				|					|
	|	getmerkleproofofuser		|						|				|				|					|
	|	(Call with User ID)			|						|				|				|					|
	|			|-----------------> |                       |               |				|					|
	|			|		 Call UserEnquiry Service			|				|				|					|
	|			|					|---------------------> |				|				|					|
	|			|					|		  Fetch User from InMemoryDB	|				|					|
	|			|					|						|-------------> |				|					|
	|			|					|						|		Return single User		|					|
	|			|					|						| <-------------|				|					|
	|			|					|		Fetch Merkle Tree from Repo		|				|					|
	|			|					|						|-----------------------------> |					|
	|			|					|						|				|		Return Merkle Tree			|
	|			|					|						| <-----------------------------|					|
	|			|					|		Fetch Merkle path from Library	|				|					|
	|			|					|						|-------------------------------------------------> |
	|			|					|						|				|				|		Return Merkle Path
	|			|					|						| <-------------------------------------------------|
	|			|					|			Return Merkle Path,			|				|					|
	|			|					|			 User ID & Balance			|				|					|
Merkle Path,<---| <-----------------| <-------------------- |				|				|					|
User ID & Bal. 	|					|						|				|				|					|
