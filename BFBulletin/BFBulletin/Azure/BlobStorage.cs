﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Azure.Storage.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;


/// <summary>
/// THIS FILE IS EXCLUDED. IT USES OLDER SYNC STYLE AND IT"S NOT NEEDED AT THIS POINT.
/// </summary>


namespace Azure.Storage
{
	/// <summary>
	/// Simple helper class for Windows Azure storage blobs
	/// </summary>
    public class BlobStorage : IBlobStorage
	{
		private readonly CloudBlobContainer cloudBlobContainer;

	    /// <summary>
	    /// Creates a new BlobStorage object
	    /// </summary>
	    /// <param name="blobContainerName">The name of the blob to be managed</param>
	    /// <param name="storageConnectionString">The connection string pointing to the storage account (this can be local or hosted in Windows Azure</param>
	    public BlobStorage(string blobContainerName, string storageConnectionString, bool isPublic = true)
	    {
	        Validate.BlobContainerName(blobContainerName, "blobContainerName");
	        Validate.String(storageConnectionString, "storageConnectionString");

	        var cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
	        var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

	        cloudBlobContainer = cloudBlobClient.GetContainerReference(blobContainerName);
	        cloudBlobContainer.CreateIfNotExists();

	        if (!isPublic)
	        {
	            return;
	        }

            var permission = cloudBlobContainer.GetPermissions();
	        permission.PublicAccess = BlobContainerPublicAccessType.Container;
	        cloudBlobContainer.SetPermissions(permission);
	    }

	    /// <summary>
		/// Creates a new block blob and populates it from a stream
		/// </summary>
		/// <param name="blobId">The blobId for the block blob</param>
		/// <param name="contentType">The content type for the block blob</param>
		/// <param name="data">The data to store in the block blob</param>
		/// <returns>The URI to the created block blob</returns>
		public string CreateBlockBlob(string blobId, string contentType, Stream data)
		{
			Validate.BlobName(blobId, "blobId");
			Validate.String(contentType, "contentType");
			Validate.Null(data, "data");

			var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobId);
			cloudBlockBlob.Properties.ContentType = contentType;
			cloudBlockBlob.UploadFromStream(data);

			return cloudBlockBlob.Uri.ToString();
		}

		/// <summary>
		/// Creates a new block blob and populates it from a byte array
		/// </summary>
		/// <param name="blobId">The blobId for the block blob</param>
		/// <param name="contentType">The content type for the block blob</param>
		/// <param name="data">The data to store in the block blob</param>
		/// <returns>The URI to the created block blob</returns>
		public string CreateBlockBlob(string blobId, string contentType, byte[] data)
		{
			Validate.BlobName(blobId, "blobId");
			Validate.String(contentType, "contentType");
			Validate.Null(data, "data");

            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobId);
			cloudBlockBlob.Properties.ContentType = contentType;
			cloudBlockBlob.UploadFromByteArray(data, 0, data.Length);

			return cloudBlockBlob.Uri.ToString();
		}

        /// <summary>
        /// Creates a new block blob and populates it from a string
        /// </summary>
        /// <param name="blobId">The blobId for the block blob</param>
        /// <param name="contentType">The content type for the block blob</param>
        /// <param name="data">The data to store in the block blob</param>
        /// <returns>The URI to the created block blob</returns>
        public string CreateBlockBlob(string blobId, string contentType, string data)
        {
            Validate.BlobName(blobId, "blobId");
            Validate.String(contentType, "contentType");
            Validate.String(data, "data");

            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobId);
            cloudBlockBlob.Properties.ContentType = contentType;
            cloudBlockBlob.UploadText(data);

            return cloudBlockBlob.Uri.ToString();
        }

	    /// <summary>
	    /// Creates a new block blob and populates it from a file
	    /// </summary>
	    /// <param name="blobId">The blobId for the block blob</param>
	    /// <param name="filePath"></param>
	    /// <returns>The URI to the created block blob</returns>
	    public string CreateBlockBlob(string blobId, string filePath)
	    {
            Validate.BlobName(blobId, "blobId");
            Validate.String(filePath, "contentType");

            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobId);
            cloudBlockBlob.UploadFromFile(filePath);

            return cloudBlockBlob.Uri.ToString();
	    }

		/// <summary>
		/// Gets a reference to a block blob with the given unique blob name
		/// </summary>
		/// <param name="blobId">The unique block blob identifier</param>
		/// <returns>A reference to the block blob</returns>
		public CloudBlockBlob GetBlockBlobReference(string blobId)
		{
			Validate.BlobName(blobId, "blobId");

			return cloudBlobContainer.GetBlockBlobReference(blobId);
		}

        /// <summary>
        /// Returns as stream with the contents of a block blob
        /// with the given blob name
        /// </summary>
        /// <param name="blobId"></param>
        /// <returns>Stream</returns>
	    public Stream GetBlockBlobDataAsStream(string blobId)
	    {
            Validate.BlobName(blobId, "blobId");

            var blob = cloudBlobContainer.GetBlockBlobReference(blobId);
            var stream = new MemoryStream();
            blob.DownloadToStream(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
	    }

        /// <summary>
        /// Returns as string with the contents of a block blob
        /// with the given blob name
        /// </summary>
        /// <param name="blobId"></param>
        /// <returns>string</returns>
	    public string GetBlockBlobDataAsString(string blobId)
	    {
            Validate.BlobName(blobId, "blobId");

            var blob = cloudBlobContainer.GetBlockBlobReference(blobId);
            return blob.DownloadText();
	    }

        /// <summary>
        /// Returns a list of all the blobs in a container
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IListBlobItem> ListBlobsInContainer()
        {
            return cloudBlobContainer.ListBlobs(null, true).ToList();
        } 

		/// <summary>
		/// Deletes the blob container
		/// </summary>
        public void DeleteBlobContainer()
        {
            cloudBlobContainer.DeleteIfExists();
        }

        /// <summary>
        /// Deletes the block blob with the given unique blob name
        /// </summary>
        /// <param name="blobId">The unique block blob identifier</param>
        public void DeleteBlob(string blobId)
        {
            var blob = cloudBlobContainer.GetBlockBlobReference(blobId);
            blob.DeleteIfExists();
        }

        /// <summary>
        /// Adds data to the end of an Append blob. Should be used within a single writer
        /// as the code is not optimised for concurrent writers
        /// </summary>
        /// <param name="blobId"></param>
        /// <param name="data"></param>
        public string AddDataToAppendBlockBlob(string blobId, string data)
        {
            var appendBlob = GetAppendBlockBlobReference(blobId);
            if(!appendBlob.Exists())
            {
                appendBlob.CreateOrReplace();
            }

            appendBlob.AppendText(data);

            return appendBlob.Uri.ToString();
        }

        /// <summary>
        /// Gets a reference to an Append blob by blob id/name
        /// </summary>
        /// <param name="blobId"></param>
        /// <returns></returns>
        public CloudAppendBlob GetAppendBlockBlobReference(string blobId)
        {
            Validate.BlobName(blobId, "blobId");

            return cloudBlobContainer.GetAppendBlobReference(blobId);
        }
    }
}